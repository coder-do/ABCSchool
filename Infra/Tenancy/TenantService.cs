using ABCSharedLibrary.Models.Requests.Tenancy;
using ABCSharedLibrary.Models.Responses.Tenancy;
using Application.Features.Tenancy;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infra.Contexts.Seeders;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Tenancy
{
    public class TenantService : ITenantService
    {
        private readonly IMultiTenantStore<ABCSchoolTenantInfo> _tenantStore;
        private readonly ApplicationDBSeeder _dbSeeder;
        private readonly IServiceProvider _serviceProvider;

        public TenantService(IMultiTenantStore<ABCSchoolTenantInfo> tenantStore, ApplicationDBSeeder dbSeeder, IServiceProvider serviceProvider)
        {
            _tenantStore = tenantStore;
            _dbSeeder = dbSeeder;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> ActivateAsync(string id)
        {
            var tenant = await _tenantStore.TryGetAsync(id);
            tenant.IsActive = true;
            
            await _tenantStore.TryUpdateAsync(tenant);
            return tenant.Identifier;
        }

        public async Task<string> CreateTenantAsync(CreateTenantRequest model, CancellationToken ct)
        {
            var newTenant = new ABCSchoolTenantInfo
            {
                Id = model.Identifier,
                Identifier = model.Identifier,
                ConnectionString = model.ConnectionString,
                Name = model.Name,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsActive = model.IsActive,
                ValidUpTo = model.ValidUpTo,
            };

            await _tenantStore.TryAddAsync(newTenant);

            using var scope = _serviceProvider.CreateScope();

            _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
                {
                    TenantInfo = newTenant,
                };

            await scope.ServiceProvider.GetRequiredService<ApplicationDBSeeder>()
                .InitializeDatabaseAsync(ct);

            return newTenant.Identifier;
        }

        public async Task<string> DeactivateAsync(string id)
        {
            var tenant = await _tenantStore.TryGetAsync(id);
            tenant.IsActive = false;

            await _tenantStore.TryUpdateAsync(tenant);
            return tenant.Identifier;
        }

        public async Task<TenantResponse> GetTenantByIdAsync(string id)
        {
            var tenantInDB = await _tenantStore.TryGetAsync(id);

            return tenantInDB.Adapt<TenantResponse>();
        }

        public async Task<List<TenantResponse>> GetTenantsAsync()
        {
            var tenants = await _tenantStore.GetAllAsync();

            return tenants.Adapt<List<TenantResponse>>();
        }

        public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest request)
        {
            var tenant = await _tenantStore.TryGetAsync(request.TenantId);
            tenant.ValidUpTo = request.NewExpiryDate;

            await _tenantStore.TryUpdateAsync(tenant);
            return tenant.Identifier;
        }
    }
}
