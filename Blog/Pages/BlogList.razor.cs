using Blog.Core;
using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Blog.Pages
{
    public class BlogListBase : ComponentBase
    {
        [Inject] BlogService BlogService { get; set; }
        protected List<BlogPosts> blogs = new();

        protected override void OnInitialized()
        {
            _ = new Timer(new TimerCallback(_ =>
            {
                InvokeAsync(async () =>
                {
                    blogs = await BlogService.GetBlogsAsync(10);
                    blogs = blogs.Where(x => x.Published).ToList();
                    StateHasChanged();
                });
            }), null, 1000, 5000);
        }
    }
}
