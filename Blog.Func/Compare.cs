using Blog.Func.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Func
{
    public class Compare
    {
        private IConfiguration Configuration { get; set; }
        private TwitterServiceV2 TwitterServiceV2 { get; set; }
        private TwitterService TwitterService { get; set; }
        private readonly List<string> users;
        public Compare(IConfiguration configuration, TwitterService TwitterService, TwitterServiceV2 TwitterServiceV2)
        {
            this.TwitterService = TwitterService;
            this.TwitterServiceV2 = TwitterServiceV2;
            Configuration = configuration;
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1"),
                "davidfowl",
                "zogface",
                "juliankay"
            };

        }

        public async Task CompareTwitterFollowers(ILogger log)
        {
            foreach (var username in users)
            {
                var oldfoll = (await TwitterService.TwitterClient.Users.GetFollowerIdsAsync(username)).Length;
                var newfoll = TwitterServiceV2.UsersModel.data.FirstOrDefault(x => x.username == username).public_metrics.followers_count;
                if (newfoll != oldfoll)
                {
                    log.LogInformation($"False {newfoll} {oldfoll}");
                }
            }
        }
    }
}
