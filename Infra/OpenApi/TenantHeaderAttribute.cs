using Infra.Tenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.OpenApi
{
    public class TenantHeaderAttribute : SwaggerHeaderAttribute
    {
        public TenantHeaderAttribute() 
            : base(TenancyConstants.TenandIdName, description: "Enter Tenant name", defaultValue: string.Empty, isRequired: true)
        { }
    }
}
