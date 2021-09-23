using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetinvi;

namespace Blog.Func.Services
{
    public class TwitterService
    {
        private readonly Chart Chart;
        private TwitterClient TwitterClient { get; set; }
        private IConfiguration Configuration { get; set; }

        private readonly List<string> users;

        public TwitterService(IConfiguration configuration, CosmosClient cosmosClient)
        {
            Configuration = configuration;
            Chart = new Chart(cosmosClient, configuration);
            TwitterClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1"),
                "davidfowl"
            };
        }

        public async Task GetTwitterFollowers(ILogger log)
        {
            foreach (var username in users)
            {
                var followers = (await TwitterClient.Users.GetFollowerIdsAsync(username)).Length;
                await Chart.SaveData(followers, 0, username);
            }
        }

        public async Task GetTwitterFollowing(ILogger log)
        {
            foreach (var username in users)
            {
                var friends = (await TwitterClient.Users.GetFriendIdsAsync(username)).Length;
                await Chart.SaveData(friends, 1, username);
            }
        }

        public async Task GetNumberOfTweets(ILogger log)
        {
            foreach (var username in users)
            {
                var friends = await TwitterClient.Users.GetUserAsync(username);
                await Chart.SaveData(friends.StatusesCount, 2, username);
            }
        }

        public async Task GetTwitterFav(ILogger log)
        {
            foreach (var username in users)
            {
                try
                {
                    var friends = await TwitterClient.Users.GetUserAsync(username);
                    await Chart.SaveData(friends.FavoritesCount, 3, username);
                }
                catch (Exception e)
                {
                    log.LogError($"Failed to save for {username} Exception {e.Message}");
                }
            }
        }

        [FunctionName("GetTwitterFav")]
        [OpenApiOperation(operationId: "GetTwitterFavFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetTwitterFavFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetTwitterFav(log);
        }

        [FunctionName("GetNumberOfTweets")]
        [OpenApiOperation(operationId: "GetNumberOfTweetsFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetNumberOfTweetsFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetNumberOfTweets(log);
        }

        [FunctionName("GetTwitterFollowers")]
        [OpenApiOperation(operationId: "GetTwitterFollowersFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetTwitterFollowersFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetTwitterFollowers(log);
        }

        [FunctionName("GetTwitterFollowing")]
        [OpenApiOperation(operationId: "GetTwitterFollowingFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetTwitterFollowingFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetTwitterFollowing(log);
        }
    }
}
