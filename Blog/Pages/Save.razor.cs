using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class SaveMetricsBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }

        [Inject] private NavigationManager UriHelper { get; set; }

        protected override void OnInitialized()
        {
            Save();
        }

        private void Save()
        {
            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
