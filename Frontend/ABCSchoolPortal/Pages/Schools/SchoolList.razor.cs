using ABCSharedLibrary.Constants;
using ABCSharedLibrary.Models.Responses.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using App.Infra.Extensions;

namespace ABCSchoolPortal.Pages.Schools
{
    public partial class SchoolList
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;

        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;

        private bool _isLoading = true;

        private bool _canCreateSchools;
        private bool _canUpdateSchools;
        private bool _canDeleteSchools;

        private List<SchoolResponse> _schoolList = [];

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthState).User;

            _canCreateSchools = await AuthService.HasPermissionAsync(user, SchoolFeature.Schools, SchoolAction.Create);
            _canUpdateSchools = await AuthService.HasPermissionAsync(user, SchoolFeature.Schools, SchoolAction.Update);
            _canDeleteSchools = await AuthService.HasPermissionAsync(user, SchoolFeature.Schools, SchoolAction.Delete);

            // Load Schools
            await LoadSchoolsAsync();
            _isLoading = false;
        }

        private async Task LoadSchoolsAsync()
        {
            var result = await _schoolService.GetAllAsync();
            if (result.IsSuccessful)
            {
                _schoolList = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }

        private async Task OnBoardNewSchoolAsync()
        {
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                BackdropClick = false
            };

            var dialog = await _dialogService.ShowAsync<CreateSchool>(title: null, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await LoadSchoolsAsync();
            }
        }

        private void Cancel()
        {
            _navigation.NavigateTo("/");
        }
    }
}
