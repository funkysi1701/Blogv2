using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Pages
{
    public class SaveMetricsBase : ComponentBase
    {
        //[Inject] private GithubService GithubService { get; set; }
        [Inject] private DevToService DevToService { get; set; }

        [Inject] private BlogService BlogService { get; set; }
        [Inject] private TwitterService TwitterService { get; set; }

        //[Inject] private PowerService PowerService { get; set; }
        [Inject] private NavigationManager UriHelper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Save();
        }

        private async Task Save()
        {
            await BlogService.GetCommits();

            //await TwitterService.GetTwitterFav();
            //await GithubService.GetGitHubStars();
            //await GithubService.GetGitHubRepo();
            //await TwitterService.GetTwitterFollowers();
            //await TwitterService.GetTwitterFollowing();
            //await TwitterService.GetNumberOfTweets();
            //await GithubService.GetGitHubFollowers();
            //await GithubService.GetGitHubFollowing();
            //await DevToService.GetDevTo();
            var r = new Random();
            var rnd = r.Next(2);
            if (rnd == 1)
            {
                //await PowerService.GetElec();
            }
            else
            {
                //await PowerService.GetGas();
            }

            UriHelper.NavigateTo("/metrics", true);
        }
    }
}
