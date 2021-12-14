using Microsoft.AspNetCore.Components;

namespace Blog.Pages
{
    public class SaveMetricsBase : ComponentBase
    {
        [Inject] private NavigationManager UriHelper { get; set; }

        protected override void OnInitialized()
        {
            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
