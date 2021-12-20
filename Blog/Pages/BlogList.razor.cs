using Blog.Core;
using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class BlogListBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }
        protected List<BlogPosts> blogs = new();

        protected override async Task OnInitializedAsync()
        {
            blogs = await BlogService.GetBlogsAsync(20);
        }
    }
}
