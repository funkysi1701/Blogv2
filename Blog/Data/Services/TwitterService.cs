using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Tweetinvi;

namespace Blog.Data.Services
{
    public class TwitterService
    {
        private readonly BlogService _service;
        private TwitterClient TwitterClient { get; set; }
        private IConfiguration Configuration { get; set; }

        public TwitterService(IConfiguration configuration, BlogService BlogService)
        {
            Configuration = configuration;
            _service = BlogService;
            TwitterClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
        }

        public async Task GetTwitterFollowers()
        {
            var followers = (await TwitterClient.Users.GetFollowerIdsAsync(Configuration.GetValue<string>("Username1"))).Length;
            await _service.SaveData(followers, 0);
        }

        public async Task GetTwitterFollowing()
        {
            var friends = (await TwitterClient.Users.GetFriendIdsAsync(Configuration.GetValue<string>("Username1"))).Length;
            await _service.SaveData(friends, 1);
        }

        public async Task GetNumberOfTweets()
        {
            var friends = await TwitterClient.Users.GetUserAsync(Configuration.GetValue<string>("Username1"));
            await _service.SaveData(friends.StatusesCount, 2);
        }

        public async Task GetTwitterFav()
        {
            var friends = await TwitterClient.Users.GetUserAsync(Configuration.GetValue<string>("Username1"));
            await _service.SaveData(friends.FavoritesCount, 3);
        }
    }
}
