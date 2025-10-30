using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Tenancy
{
    public interface ITenantDBSeeder
    {
        Task InitializeDatabaseAsync(CancellationToken ct);
    }
}
