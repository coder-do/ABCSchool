using System.Collections.ObjectModel;

namespace ABCSharedLibrary.Constants
{
    public static class SchoolAction
    {
        public const string Read = nameof(Read);
        public const string Create = nameof(Create);
        public const string Update = nameof(Update);
        public const string Delete = nameof(Delete);
        public const string RefreshToken = nameof(RefreshToken);
        public const string UpgradeSubscription = nameof(UpgradeSubscription);
    }

    public static class SchoolFeature
    {
        public const string Tenants = nameof(Tenants);
        public const string Users = nameof(Users);
        public const string Roles = nameof(Roles);
        public const string UserRoles = nameof(UserRoles);
        public const string RoleClaims = nameof(RoleClaims);
        public const string Schools = nameof(Schools);
        public const string Tokens = nameof(Tokens);
    }

    public record SchoolPermission(string Action, string Feature, string Description, string Group, bool IsBasic = false, bool IsRoot = false)
    {
        public string Name = NameFor(Action, Feature);
        public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
    }

    public static class SchoolPermissions
    {
        private static readonly SchoolPermission[] _allPermissions =
        [
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Tenants, "Create Tenants", "Tenancy", IsRoot: true),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Tenants, "Read Tenants", "Tenancy", IsRoot: true),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Tenants, "Update Tenants", "Tenancy", IsRoot: true),
            new SchoolPermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants, "Update Tenants Subscription", "Tenancy", IsRoot: true),

            new SchoolPermission(SchoolAction.Create, SchoolFeature.Users, "Create Users", "SystemAccess"),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Users, "Read Users", "SystemAccess"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Users, "Update Users", "SystemAccess"),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Users, "Delete Users", "SystemAccess"),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.UserRoles, "Read User Roles", "SystemAccess"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.UserRoles, "Update User Roles", "SystemAccess"),

            new SchoolPermission(SchoolAction.Create, SchoolFeature.Roles, "Create Roles", "SystemAccess"),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Roles, "Read Roles", "SystemAccess"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Roles, "Update Roles", "SystemAccess"),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Roles, "Delete Roles", "SystemAccess"),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.RoleClaims, "Read User Role Claims", "SystemAccess"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.RoleClaims, "Update User Role Claims", "SystemAccess"),

            new SchoolPermission(SchoolAction.Create, SchoolFeature.Schools, "Create Schools", "Academics", IsBasic: true),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Schools, "Read Schools", "Academics"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Schools, "Update Schools", "Academics"),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Schools, "Delete Schools", "Academics"),

            new SchoolPermission(SchoolAction.RefreshToken, SchoolFeature.Tokens, "Generate RefreshToken", "SystemAccess", IsBasic: true),
        ];

        public static IReadOnlyList<SchoolPermission> All
            = new ReadOnlyCollection<SchoolPermission>(_allPermissions);

        public static IReadOnlyList<SchoolPermission> Root
            = new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => p.IsRoot).ToArray());

        public static IReadOnlyList<SchoolPermission> Admin
            = new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => !p.IsRoot).ToArray());

        public static IReadOnlyList<SchoolPermission> Basic
            = new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(p => p.IsBasic).ToArray());
    }
}
