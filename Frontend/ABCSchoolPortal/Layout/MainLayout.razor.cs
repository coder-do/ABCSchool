using ABCSchoolPortal.Components;
using MudBlazor;

namespace ABCSchoolPortal.Layout
{
    public partial class MainLayout
    {
        private bool _drawerOpen = true;
        protected override void OnInitialized()
        {
            _interceptor.RegisterEvent();
            StateHasChanged();
        }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task LogoutDialog()
        {
            var parameters = new DialogParameters
            {
                { nameof(Logout.Title),  "Logout"},
                { nameof(Logout.ConfirmationMessage),  "Are you sure you want to logout?"},
                { nameof(Logout.ButtonText),  "Sign Out"},
                { nameof(Logout.Color),  Color.Success}
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true,
                BackdropClick = true,
            };

            await _dialogService.ShowAsync<Logout>(title: null, parameters: parameters, options: options);
        }
    }
}
