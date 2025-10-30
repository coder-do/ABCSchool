using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Tenancy
{
    public class TenancyConstants
    {
        public const string TenandIdName = "tenant";

        public const string DefaultPassword = "Admin@123";
        public const string FirstName = "admin";
        public const string LastName = "admin";

        public static class Root
        {
            public const string Id = "root";
            public const string Name = "Root";
            public const string Email = "admin.root@abcschool.com";
        }
    }
}
