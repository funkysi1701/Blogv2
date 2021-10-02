using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SocialOpinionAPI.Core;
using SocialOpinionAPI.Models.Users;
using SocialOpinionAPI.Services.Likes;
using SocialOpinionAPI.Services.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Func.Services
{
    public class TwitterServiceV2
    {
        private readonly Chart Chart;
        private UsersModel UserModel { get; set; }
        private LikesService Likes { get; set; }
        private IConfiguration Configuration { get; set; }

        private readonly List<string> users;

        public TwitterServiceV2(IConfiguration configuration, CosmosClient cosmosClient)
        {
            Configuration = configuration;
            Chart = new Chart(cosmosClient, configuration);
            
            
            OAuthInfo oAuthInfo = new OAuthInfo
            {
                AccessSecret = configuration.GetValue<string>("TWAccessSecret"),
                AccessToken = configuration.GetValue<string>("TWAccessToken"),
                ConsumerSecret = configuration.GetValue<string>("TWConsumerSecret"),
                ConsumerKey = configuration.GetValue<string>("TWConsumerKey")
            };
            var myUserService = new UserService(oAuthInfo);
            Likes = new LikesService(oAuthInfo);
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1"),
                "davidfowl",
                "zogface",
                "juliankay"
            };
            UserModel = myUserService.GetUsers(users);
            
        }

        public async Task GetTwitterFollowers(ILogger log)
        {
            foreach (var user in UserModel.data)
            {
                await Chart.SaveData(user.public_metrics.followers_count, 0, user.username);
            }  
        }

        public async Task GetTwitterFollowing(ILogger log)
        {
            foreach (var user in UserModel.data)
            {
                await Chart.SaveData(user.public_metrics.following_count, 1, user.username);
            }
        }

        public async Task GetNumberOfTweets(ILogger log)
        {
            foreach (var user in UserModel.data)
            {
                await Chart.SaveData(user.public_metrics.tweet_count, 2, user.username);
            }
        }

        public async Task GetTwitterFav(ILogger log)
        {
            foreach (var user in UserModel.data)
            {
                var listlikes = Likes.GetUsersLikedTweets(user.id, 10, 10);
                await Chart.SaveData(listlikes.Count, 3, user.username);
            }
        }

        [FunctionName("GetTwitterFavV2")]
        [OpenApiOperation(operationId: "GetTwitterFavFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetTwitterFavFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetTwitterFav(log);
        }

        [FunctionName("GetNumberOfTweetsV2")]
        [OpenApiOperation(operationId: "GetNumberOfTweetsFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetNumberOfTweetsFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetNumberOfTweets(log);
        }

        [FunctionName("GetTwitterFollowersV2")]
        [OpenApiOperation(operationId: "GetTwitterFollowersFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetTwitterFollowersFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetTwitterFollowers(log);
        }

        [FunctionName("GetTwitterFollowingV2")]
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
