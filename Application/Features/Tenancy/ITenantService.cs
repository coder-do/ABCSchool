using ABCSharedLibrary.Models.Requests.Tenancy;
using ABCSharedLibrary.Models.Responses.Tenancy;

namespace Application.Features.Tenancy
{
    public interface ITenantService
    {
        Task<string> CreateTenantAsync(CreateTenantRequest model, CancellationToken ct);
        Task<string> ActivateAsync(string id);
        Task<string> DeactivateAsync(string id);
        Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest request);
        Task<List<TenantResponse>> GetTenantsAsync();
        Task<TenantResponse> GetTenantByIdAsync(string id);
    }
}
