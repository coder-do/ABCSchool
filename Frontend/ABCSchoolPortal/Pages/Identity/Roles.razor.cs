using ABCSharedLibrary.Constants;
using ABCSharedLibrary.Models.Responses.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using App.Infra.Extensions;
using MudBlazor;

namespace ABCSchoolPortal.Pages.Identity
{
    public partial class Roles
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;

        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;

        private List<RoleResponse> _roleList = [];

        private bool _isLoading = true;

        private bool _canCreateRoles;
        private bool _canUpdateRoles;
        private bool _canDeleteRoles;
        private bool _canViewRolePermissions;

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;

            _canCreateRoles = await AuthService.HasPermissionAsync(user, SchoolFeature.Roles, SchoolAction.Create);
            _canUpdateRoles = await AuthService.HasPermissionAsync(user, SchoolFeature.Roles, SchoolAction.Update);
            _canDeleteRoles = await AuthService.HasPermissionAsync(user, SchoolFeature.Roles, SchoolAction.Delete);
            _canViewRolePermissions = await AuthService.HasPermissionAsync(user, SchoolFeature.RoleClaims, SchoolAction.Read);

            await LoadRolesAsync();
            _isLoading = false;
        }

        private async Task LoadRolesAsync()
        {
            var result = await _roleService.GetRolesAsync();
            if (result.IsSuccessful)
            {
                _roleList = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }

        private void ViewPermissions(string roleId)
        {
            _navigation.NavigateTo($"/role-permissions/{roleId}");
        }

        private void Cancel()
        {
            _navigation.NavigateTo("/");
        }
    }
}
