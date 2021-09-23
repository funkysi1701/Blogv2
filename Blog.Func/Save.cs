using Blog.Func.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blog.Func
{
    public class Save
    {
        private readonly TwitterService twitterService;
        public Save(TwitterService twitterService)
        {
            this.twitterService = twitterService;
        }

        [FunctionName("SaveData")]
        public async Task Run([TimerTrigger("0 59 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFav(log);
            await twitterService.GetTwitterFollowers(log);
            await twitterService.GetTwitterFollowing(log);
            await twitterService.GetNumberOfTweets(log);
        }
    }
}
