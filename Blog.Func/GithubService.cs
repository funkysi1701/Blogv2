using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Func
{
    public class GithubService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }

        public GithubService(IConfiguration configuration, CosmosClient cosmosClient)
        {
            Configuration = configuration;
            Chart = new Chart(cosmosClient, this);
        }

        public async Task GetGitHubFollowers()
        {
            var github = GitHub();
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await Chart.SaveData(user.Followers, 4);
        }

        public async Task GetGitHubFollowing()
        {
            var github = GitHub();
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await Chart.SaveData(user.Following, 5);
        }

        public async Task GetGitHubRepo()
        {
            var github = GitHub();
            var user = await github.User.Get(Configuration.GetValue<string>("Username1"));
            await Chart.SaveData(user.PublicRepos, 6);
        }

        public async Task GetGitHubStars()
        {
            var github = GitHub();
            var stars = await github.Activity.Starring.GetAllForCurrent();
            await Chart.SaveData(stars.Count, 7);
        }

        public async Task GetCommits()
        {
            var github = GitHub();
            try
            {
                var events = await github.Activity.Events.GetAllUserPerformed("funkysi1701");
                var today = events.Where(x => x.Type == "PushEvent" && x.CreatedAt > DateTime.Now.Date).ToList();
                var sofar = await Chart.GetAll();
                sofar = sofar.Where(x => x.Date != null && x.Type == 8 && x.Date < DateTime.Now.Date).OrderBy(y => y.Date).ToList();
                if (sofar.Count == 0)
                {
                    await Chart.SaveData(today.Count, 8);
                }
                else await Chart.SaveData(today.Count + sofar.Last().Value.Value, 8);
            }
            catch (Exception e)
            {
            }
        }

        public GitHubClient GitHub()
        {
            var github = new GitHubClient(new ProductHeaderValue("funkysi1701"));
            var tokenAuth = new Credentials("55ba1de1d50accfebfd9c421e9156564712c4147");
            github.Credentials = tokenAuth;
            return github;
        }
    }
}
