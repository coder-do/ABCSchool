using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infra.Contexts.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Tenancy
{
    public class TenantDbSeeder : ITenantDBSeeder
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly IServiceProvider _serviceProvider;
        public TenantDbSeeder(TenantDbContext tenantDbContext, IServiceProvider serviceProvider)
        {
            _tenantDbContext = tenantDbContext;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeDatabaseAsync(CancellationToken ct)
        {
            await InitializeDatabaseWithTenantAsync(ct);

            foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(ct))
            {
                await InititalizeApplicationDbForTenantAsync(tenant, ct);
            }
        }

        private async Task InititalizeApplicationDbForTenantAsync(ABCSchoolTenantInfo tenant, CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();

            _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
                {
                    TenantInfo = tenant,
                };

            await scope.ServiceProvider.GetRequiredService<ApplicationDBSeeder>()
                .InitializeDatabaseAsync(ct);
        }

        private async Task InitializeDatabaseWithTenantAsync(CancellationToken ct)
        {
            if (await _tenantDbContext.TenantInfo.FindAsync([TenancyConstants.Root.Id], ct) is null)
            {
                var rootTenant = new ABCSchoolTenantInfo
                {
                    Id = TenancyConstants.Root.Id,
                    Identifier = TenancyConstants.Root.Id,
                    Name = TenancyConstants.Root.Name,
                    Email = TenancyConstants.Root.Email,
                    FirstName = TenancyConstants.FirstName,
                    LastName = TenancyConstants.LastName,
                    IsActive = true,
                    ValidUpTo = DateTime.UtcNow.AddYears(2),
                };

                await _tenantDbContext.TenantInfo.AddAsync(rootTenant, ct);
                await _tenantDbContext.SaveChangesAsync(ct);
            }
        }
    }
}
