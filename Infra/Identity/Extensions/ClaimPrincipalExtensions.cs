using Infra.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Identity.Extensions
{
    public static class ClaimPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.Email);
        public static string GetUserId(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.NameIdentifier);
        public static string GetTenant(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimConstants.Tenant);
        public static string GetFirstname(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.Name);
        public static string GetLastname(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.Surname);
        public static string GetPhoneNumber(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.MobilePhone);
    }
}
