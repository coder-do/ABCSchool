using ABCSharedLibrary.Models.Requests.Tenancy;
using Application.Features.Tenancy;
using Application.Features.Tenancy.Commands;
using Application.Features.Tenancy.Queries;
using Infra.Constants;
using Infra.Identity.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TenantsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Tenants)]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantRequest request)
        {
            var response = await Sender.Send(new CreateTenantCommand { CreateTenant = request });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPut("{tenantId}/activate")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Tenants)]
        public async Task<IActionResult> ActivateTenantAsync(string tenantId)
        {
            var response = await Sender.Send(new ActivateTenantCommand { TenantId = tenantId });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPut("{tenantId}/deactivate")]
        [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Tenants)]
        public async Task<IActionResult> DeactivateTenantAsync(string tenantId)
        {
            var response = await Sender.Send(new DeactivateTenantCommand { TenantId = tenantId });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPut("upgrade")]
        [ShouldHavePermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants)]
        public async Task<IActionResult> UpdateSubscriptionAsync([FromBody] UpdateTenantSubscriptionRequest request)
        {
            var response = await Sender.Send(new UpdateTenantSubscriptionCommand { UpdateTenantSubscriptionRequest = request });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet("{tenantId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Tenants)]
        public async Task<IActionResult> GetTenantByIdAsync(string tenantId)
        {
            var response = await Sender.Send(new GetTenantByIdQuery { TenantId = tenantId });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Tenants)]
        public async Task<IActionResult> GetAllTenantsAsync()
        {
            var response = await Sender.Send(new GetTenantsQuery());

            if (response.IsSuccessful)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
