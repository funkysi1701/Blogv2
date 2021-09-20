using Microsoft.AspNetCore.Components;

namespace Blog.Pages
{
    public class MetricsBase : ComponentBase
    {
        [Parameter]
        public int OffSet { get; set; } = 0;
    }
}
