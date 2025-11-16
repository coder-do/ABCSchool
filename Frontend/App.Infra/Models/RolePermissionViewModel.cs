using ABCSharedLibrary.Constants;

namespace App.Infra.Models
{
    public record RolePermissionViewModel : SchoolPermission
    {
        public RolePermissionViewModel(
            string Action,
            string Feature,
            string Description,
            string Group,
            bool IsBasic = false,
            bool IsRoot = false)
            : base(Action, Feature, Description, Group, IsBasic, IsRoot)
        {
        }

        public bool IsSelected { get; set; }
    }
}
