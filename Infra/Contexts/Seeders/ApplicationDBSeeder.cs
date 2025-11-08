using Finbuckle.MultiTenant.Abstractions;
using Infra.Constants;
using Infra.Identity.Models;
using Infra.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Contexts.Seeders
{
    public class ApplicationDBSeeder
    {
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDBContext _applicationDbContext;

        public ApplicationDBSeeder(
            IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor,
            RoleManager<ApplicationRole> roleManager, 
            UserManager<ApplicationUser> userManager, 
            ApplicationDBContext applicationDbContext)
        {
            _tenantContextAccessor = tenantContextAccessor;
            _roleManager = roleManager;
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            if (_applicationDbContext.Database.GetMigrations().Any())
            {
                if ((await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    await _applicationDbContext.Database.MigrateAsync(cancellationToken);
                }

                
                if (await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
                {
                    await InitializeDefaultRolesAsync(cancellationToken);

                    await InitializeAdminUserAsync();
                }
            }
        }

        private async Task InitializeAdminUserAsync()
        {
            var email = _tenantContextAccessor.MultiTenantContext.TenantInfo.Email;

            if (string.IsNullOrEmpty(email)) return;

            if (await _userManager.Users
                .SingleOrDefaultAsync(u => u.Email == email) is not ApplicationUser incomingUser)
            {
                incomingUser = new ApplicationUser
                {
                    FirstName = _tenantContextAccessor.MultiTenantContext.TenantInfo.FirstName,
                    LastName = _tenantContextAccessor.MultiTenantContext.TenantInfo.LastName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    NormalizedEmail = email.ToUpperInvariant(),
                    NormalizedUserName = email.ToUpperInvariant(),
                    IsActive = true,
                };

                var password = new PasswordHasher<ApplicationUser>();

                incomingUser.PasswordHash = password.HashPassword(incomingUser, TenancyConstants.DefaultPassword);

                await _userManager.CreateAsync(incomingUser);
            }

            if (!await _userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
            {
                await _userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
            }
        }

        private async Task InitializeDefaultRolesAsync(CancellationToken ct)
        {
            foreach (var role in RoleConstants.DefaultRoles)
            {
                if (await _roleManager.Roles.SingleOrDefaultAsync(e => e.Name == role, ct) is not ApplicationRole incomingRole)
                {
                    incomingRole = new ApplicationRole()
                    {
                        Name = role,
                        Description = $"{role} Role"
                    };

                    await _roleManager.CreateAsync(incomingRole);
                }

                
                if (role == RoleConstants.Admin)
                {
                    await AsignPermissionsToRoleAsync(SchoolPermissions.Admin, incomingRole, ct);

                    if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                    {
                        await AsignPermissionsToRoleAsync(SchoolPermissions.Root, incomingRole, ct);
                    }
                }
                else if (role == RoleConstants.Basic)
                {
                    await AsignPermissionsToRoleAsync(SchoolPermissions.Basic, incomingRole, ct);
                }
            }
        }

        private async Task AsignPermissionsToRoleAsync(IReadOnlyList<SchoolPermission> permissions, ApplicationRole role, CancellationToken ct)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in permissions)
            {
                if (!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == permission.Name))
                {
                    await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = ClaimConstants.Permission,
                        ClaimValue = permission.Name,
                        Description = permission.Description,
                        Group = permission.Group
                    }, ct);

                    await _applicationDbContext.SaveChangesAsync(ct);
                }
            }
        }
    }
}
