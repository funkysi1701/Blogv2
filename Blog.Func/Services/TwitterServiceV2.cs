﻿using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SocialOpinionAPI.Core;
using SocialOpinionAPI.Models.Users;
using SocialOpinionAPI.Services.Likes;
using SocialOpinionAPI.Services.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Func.Services
{
    public class TwitterServiceV2
    {
        private readonly Chart Chart;
        public UsersModel UsersModel { get; set; }
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
            try
            {
                UsersModel = myUserService.GetUsers(users);
            }
            catch (Exception ex)
            {

            }
            
            
        }

        public async Task GetTwitterFollowers(ILogger log)
        {
            foreach (var user in UsersModel.data)
            {
                await Chart.SaveData(user.public_metrics.followers_count, 0, user.username);
            }  
        }

        public async Task GetTwitterFollowing(ILogger log)
        {
            foreach (var user in UsersModel.data)
            {
                await Chart.SaveData(user.public_metrics.following_count, 1, user.username);
            }
        }

        public async Task GetNumberOfTweets(ILogger log)
        {
            foreach (var user in UsersModel.data)
            {
                await Chart.SaveData(user.public_metrics.tweet_count, 2, user.username);
            }
        }

        public async Task GetTwitterFav(ILogger log)
        {
            foreach (var user in UsersModel.data)
            {
                var listlikes = Likes.GetUsersLikedTweets(user.id, 10, 10);
                await Chart.SaveData(listlikes.Count, 3, user.username);
            }
        }
    }
}
