using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infra.Constants;
using Infra.Identity.Extensions;
using Infra.Identity.Models;
using Infra.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infra.Identity.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtSettings _jwtSetings;
        private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _multiTenantContextAccessor;

        public TokenService(
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager,
            IOptions<JwtSettings> jwtSetings,
            IMultiTenantContextAccessor<ABCSchoolTenantInfo> multiTenantContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSetings = jwtSetings.Value;
            _multiTenantContextAccessor = multiTenantContextAccessor;
        }

        public async Task<TokenResponse> Login(TokenRequest request)
        {
            if (!_multiTenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            {
                throw new UnAuthorizedException(["Tenant subscription is not active"]);
            }

            var userInDb = await _userManager.FindByNameAsync(request.UserName)
                ?? throw new UnAuthorizedException(["Authentication not successful"]);

            if (!(await _userManager.CheckPasswordAsync(userInDb, request.Password)))
            {
                throw new UnAuthorizedException(["Incorrect username or password"]);
            }

            if (!userInDb.IsActive)
            {
                throw new UnAuthorizedException(["This user is not active"]);
            }

            if (_multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id)
            {
                if (_multiTenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
                {
                    throw new UnAuthorizedException(["Tenant subscription is expired"]);
                }
            }

            return await GenerateJwtTokenAndUpdateUserAsync(userInDb);
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var userPrincipal = GetClaimsPrincipalFromExpiringToken(request.CurrentJwt);
            var userEmail = userPrincipal.GetEmail();

            var userInDb = await _userManager.FindByEmailAsync(userEmail)
                ?? throw new UnAuthorizedException(["Authentication failed"]);

            if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnAuthorizedException(["Invalid token"]);
            }

            return await GenerateJwtTokenAndUpdateUserAsync(userInDb);
        }

        private ClaimsPrincipal GetClaimsPrincipalFromExpiringToken(string expiringToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true, // Allow expired token for refresh
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };

            var principal = tokenHandler.ValidateToken(expiringToken, validationParams, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnAuthorizedException(["Invalid token provided. Failed to generate a new token"]);
            }

            return principal;
        }

        private async Task<TokenResponse> GenerateJwtTokenAndUpdateUserAsync(ApplicationUser user)
        {
            var newJwtToken = await GenerateToken(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSetings.RefreshTokenExpiryInDays);

            await _userManager.UpdateAsync(user);

            return new TokenResponse
            {
                Jwt = newJwtToken,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
            };
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            return GenerateEncryptedToken(GenerateSigningCredentials(), await GetUserClaims(user));
        }

        private string GenerateEncryptedToken(SigningCredentials credentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSetings.TokenExpiryInMinutes),
                    signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GenerateSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_jwtSetings.Secret);

            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var role in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var currentRole = await _roleManager.FindByNameAsync(role);

                var permissionsForRole = await _roleManager.GetClaimsAsync(currentRole);

                permissionClaims.AddRange(permissionsForRole);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimConstants.Tenant, _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            }
            .Union(roleClaims)
            .Union(userClaims)
            .Union(permissionClaims);

            return claims;
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];

            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber)
                        .TrimEnd('=')
                        .Replace('+', '-')
                        .Replace('/', '_');
        }
    }
}
