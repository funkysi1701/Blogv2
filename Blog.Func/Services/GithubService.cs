using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Func.Services
{
    public class GithubService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }
        private readonly List<string> users;

        public GithubService(IConfiguration configuration, CosmosClient cosmosClient)
        {
            Configuration = configuration;
            Chart = new Chart(cosmosClient);
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1"),
                "davidfowl"
            };
        }

        public GitHubClient GitHub()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            return github;
        }

        public async Task GetGitHubFollowers()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var user = await github.User.Get(username);
                await Chart.SaveData(user.Followers, 4, username);
            }
        }

        public async Task GetGitHubFollowing()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var user = await github.User.Get(username);
                await Chart.SaveData(user.Following, 5, username);
            }
        }

        public async Task GetGitHubRepo()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var user = await github.User.Get(username);
                await Chart.SaveData(user.PublicRepos, 6, username);
            }
        }

        public async Task GetGitHubStars()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var stars = await github.Activity.Starring.GetAllForUser(username);
                await Chart.SaveData(stars.Count, 7, username);
            }
        }

        public async Task GetCommits()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var events = await github.Activity.Events.GetAllUserPerformed(username);
                var today = events.Where(x => x.Type == "PushEvent" && x.CreatedAt > DateTime.Now.Date).ToList();
                var sofar = await Chart.GetAll();
                sofar = sofar.Where(x => x.Date != null && x.Type == 8 && x.Date < DateTime.Now.Date).OrderBy(y => y.Date).ToList();
                if (sofar.Count == 0)
                {
                    await Chart.SaveData(today.Count, 8, username);
                }
                else await Chart.SaveData(today.Count + sofar.Last().Value.Value, 8, username);
            }
        }

        [FunctionName("GetCommits")]
        [OpenApiOperation(operationId: "GetCommitsFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetCommitsFn(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetCommits();
        }

        [FunctionName("GetGitHubStars")]
        [OpenApiOperation(operationId: "GetGitHubStarsFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetGitHubStarsFn(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetGitHubStars();
        }

        [FunctionName("GetGitHubRepo")]
        [OpenApiOperation(operationId: "GetGitHubRepoFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetGitHubRepoFn(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetGitHubRepo();
        }

        [FunctionName("GetGitHubFollowers")]
        [OpenApiOperation(operationId: "GetGitHubFollowersFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetGitHubFollowersFn(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetGitHubFollowers();
        }

        [FunctionName("GetGitHubFollowing")]
        [OpenApiOperation(operationId: "GetGitHubFollowingFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetGitHubFollowingFn(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetGitHubFollowing();
        }
    }
}
