using Application.Features.Identity.Tokens;
using Application.Features.Identity.Tokens.Queries;
using Infra.Constants;
using Infra.Identity.Auth;
using Infra.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        [TenantHeader]
        [OpenApiOperation("Used for login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest request)
        {
            var req = new GetTokenQuery { TokenRequest = request };
            var response = await Sender.Send(req);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("refresh-token")]
        [OpenApiOperation("Used for getting refresh token")]
        [ShouldHavePermission(SchoolAction.RefreshToken, feature: SchoolFeature.Tokens)]
        public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            var req = new GetRefreshTokenQuery { RefreshToken = request };
            var response = await Sender.Send(req);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
