using Microsoft.AspNetCore.Components;

namespace ABCSchoolPortal.Components
{
    public partial class Header
    {
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public EventCallback OnReturn { get; set; }

        protected async Task ReturnButtonClicked()
        {
            await OnReturn.InvokeAsync();
            Console.WriteLine("Return Button Clicked");
        }
    }
}
