using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class SaveMetricsBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }

        [Inject] private NavigationManager UriHelper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Save();
        }

        private async Task Save()
        {
            await BlogService.GetCommits();
            await BlogService.GetGitHubStars();
            await BlogService.GetGitHubRepo();
            await BlogService.GetGitHubFollowers();
            await BlogService.GetGitHubFollowing();

            await BlogService.GetTwitterFav();
            await BlogService.GetTwitterFollowers();
            await BlogService.GetTwitterFollowing();
            await BlogService.GetNumberOfTweets();

            await BlogService.GetDevTo();
            var r = new Random();
            var rnd = r.Next(2);
            if (rnd == 1)
            {
                await BlogService.GetElec();
            }
            else
            {
                await BlogService.GetGas();
            }

            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
