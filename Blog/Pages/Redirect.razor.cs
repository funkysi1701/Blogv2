using Blog.Core;
using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class RedirectBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }
        [Inject] private NavigationManager MyNavigationManager { get; set; }

        protected List<BlogPosts> blogs;

        [Parameter]
        public int Year { get; set; }

        [Parameter]
        public int Month { get; set; }

        [Parameter]
        public int Day { get; set; }

        [Parameter]
        public string Title { get; set; }

        protected override async Task OnInitializedAsync()
        {
            blogs = await BlogService.GetBlogsAsync(100);
            if (Year > 0)
            {
                blogs = blogs.Where(x => x.Published_At.Value.Year == Year).ToList();
            }
            if (Month > 0)
            {
                blogs = blogs.Where(x => x.Published_At.Value.Month == Month).ToList();
            }
            if (Day > 0)
            {
                blogs = blogs.Where(x => x.Published_At.Value.Day == Day).ToList();
            }
        }
    }
}
