using ABCSharedLibrary.Models.Requests.User;
using App.Infra.Extensions;
using MudBlazor;

namespace ABCSchoolPortal.Pages.Identity
{
    public partial class Profile
    {
        private UpdateUserRequest UpdateUserRequest { get; set; } = new();
        private string Firstname { get; set; }
        private string Lastname { get; set; }
        private char FirstLetterOfFirstname { get; set; }
        private string Email { get; set; }

        public string UserId { get; set; }
        private MudForm _form;
        protected override async Task OnInitializedAsync()
        {
            await SetCurrentUserDetails();
        }

        private async Task SetCurrentUserDetails()
        {
            var state = await _applicationStateProvider.GetAuthenticationStateAsync();

            var user = state.User;

            Firstname = user.GetFirstname();
            Lastname = user.GetLastname();
            Email = user.GetEmail();
            UserId = user.GetUserId();

            UpdateUserRequest.Id = UserId;
            UpdateUserRequest.FirstName = Firstname;
            UpdateUserRequest.LastName = Lastname;
            UpdateUserRequest.PhoneNumber = user.GetPhoneNumber();

            if (Firstname.Length > 0)
            {
                FirstLetterOfFirstname = Firstname[0];
            }
        }

        private async Task UpdateUserDetailsAsync()
        {
            var result = await _userService.UpdateUserAsync(UpdateUserRequest);

            if (result.IsSuccessful)
            {
                await _tokenService.LogoutAsync();
                _snackbar.Add("Your profile has been updated. Login again.", Severity.Info);
                _navigation.NavigateTo("/");
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }
    }
}
