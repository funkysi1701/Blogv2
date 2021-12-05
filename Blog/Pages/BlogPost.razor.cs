using Blog.Core;
using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class BlogPostBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }

        protected BlogPosts thisblog;
        protected BlogPostsSingle thisblogsingle;

        [Parameter]
        public string Slug { get; set; }

        protected override async Task OnInitializedAsync()
        {
            List<BlogPosts> blogs = await BlogService.GetBlogsAsync(200);

            var url = HttpContextAccessor.HttpContext.Request.Path.Value;
            if (url.Contains("lone-developer-to-senior-developer-my-2021-story-c3d"))
            {
                HttpContextAccessor.HttpContext.Response.Redirect("/posts/lone -developer-to-senior-developer-my-2021-story-3g0a"); 
            }

                thisblog = blogs.FirstOrDefault(x => x.Slug == Slug && x.Published);
            if (thisblog != null)
            {
                thisblogsingle = await BlogService.GetBlogPostAsync(thisblog.Id);
            }

            this.StateHasChanged();
        }
    }
}
