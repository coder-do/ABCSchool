using ABCSchoolPortal.Components;
using ABCSharedLibrary.Models.Requests.User;
using ABCSharedLibrary.Models.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ABCSchoolPortal.Pages.Identity
{
    public partial class Users
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; } = default!;

        [Inject]
        protected IAuthorizationService AuthService { get; set; } = default!;

        private List<UserResponse> _userList = [];

        private bool _isLoading = true;
        private bool _canCreateUsers;
        private bool _canViewRoles;

        protected override async Task OnInitializedAsync()
        {
            await LoadUsers();
            _isLoading = false;
        }

        private async Task LoadUsers()
        {
            var result = await _userService.GetUsersAsync();
            if (result.IsSuccessful)
            {
                _userList = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeUserRegistrationDialog()
        {
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                BackdropClick = false,
                FullWidth = true
            };

            var dialog = await _dialogService.ShowAsync<RegisterUser>(title: null, options: options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await LoadUsers();
            }
        }

        private async Task ActivateOrDeactivativeAsync(UserResponse user)
        {
            if (user.IsActive)
            {
                // Deactivate
                var parameters = new DialogParameters
                {
                    { nameof(Confirmation.Title), "Deactivate User" },
                    { nameof(Confirmation.Message), $"Are you sure you want to Deactivate '{user.FirstName} {user.LastName}'?" },
                    { nameof(Confirmation.ButtonText), "Deactivate" },
                    { nameof(Confirmation.Color), Color.Error },
                    { nameof(Confirmation.InputIcon), Icons.Material.Filled.Person }
                };

                var options = new DialogOptions
                {
                    CloseButton = true,
                    MaxWidth = MaxWidth.Small,
                    BackdropClick = true,
                    FullWidth = true
                };

                var dialog = await _dialogService.ShowAsync<Confirmation>(title: null, parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    var response = await _userService.ChangeUserStatusAsync(new ChangeUserStatusRequest
                    {
                        UserId = user.Id,
                        Activation = false
                    });

                    if (response.IsSuccessful)
                    {
                        _snackbar.Add(response.Messages[0], Severity.Success);

                        await LoadUsers();
                    }
                    else
                    {
                        foreach (var message in response.Messages)
                        {
                            _snackbar.Add(message, Severity.Error);
                        }
                    }
                }
            }
            else
            {
                // Activate
                var parameters = new DialogParameters
                {
                    { nameof(Confirmation.Title), "Activate User" },
                    { nameof(Confirmation.Message), $"Are you sure you want to Activate '{user.FirstName} {user.LastName}'?" },
                    { nameof(Confirmation.ButtonText), "Activate" },
                    { nameof(Confirmation.Color), Color.Primary },
                    { nameof(Confirmation.InputIcon), Icons.Material.Filled.Person }
                };

                var options = new DialogOptions
                {
                    CloseButton = true,
                    MaxWidth = MaxWidth.Small,
                    BackdropClick = true,
                    FullWidth = true
                };

                var dialog = await _dialogService.ShowAsync<Confirmation>(title: null, parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    var response = await _userService.ChangeUserStatusAsync(new ChangeUserStatusRequest
                    {
                        UserId = user.Id,
                        Activation = true
                    });

                    if (response.IsSuccessful)
                    {
                        _snackbar.Add(response.Messages[0], Severity.Success);

                        await LoadUsers();
                    }
                    else
                    {
                        foreach (var message in response.Messages)
                        {
                            _snackbar.Add(message, Severity.Error);
                        }
                    }
                }
            }
        }

        private void Cancel()
        {
            _navigation.NavigateTo("/");
        }

        private void GoToRoles(string userId)
        {
            _navigation.NavigateTo($"/user-roles/{userId}");
        }
    }
}
