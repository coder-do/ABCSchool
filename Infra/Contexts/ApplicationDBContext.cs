using Domain.Entities;
using Finbuckle.MultiTenant.Abstractions;
using Infra.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infra.Contexts
{
    public class ApplicationDBContext : BaseDBContext
    {
        public ApplicationDBContext(
                IMultiTenantContextAccessor<ABCSchoolTenantInfo> multiTenantContextAccessor, 
                DbContextOptions<ApplicationDBContext> options) 
            : base(multiTenantContextAccessor, options)
        { }

        public DbSet<School> Schools => Set<School>();
    }
}
