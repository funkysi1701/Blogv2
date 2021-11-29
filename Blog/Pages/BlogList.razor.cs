using Blog.Core;
using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class BlogListBase : ComponentBase
    {
        [Inject] BlogService BlogService { get; set; }
        protected List<BlogPosts> blogs = new();

        protected override async Task OnInitializedAsync()
        {
            blogs = await BlogService.GetBlogsAsync(20);
        }
    }
}
