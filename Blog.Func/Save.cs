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
        private readonly PowerService powerService;
        private readonly GithubService githubService;
        private readonly DevToService devToService;
        public Save(TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService)
        {
            this.twitterService = twitterService;
            this.powerService = powerService;
            this.githubService = githubService;
            this.devToService = devToService;
        }

        [FunctionName("SaveData")]
        public async Task Run([TimerTrigger("0 59 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFav(log);
            await twitterService.GetTwitterFollowers(log);
            await twitterService.GetTwitterFollowing(log);
            await twitterService.GetNumberOfTweets(log);
            await powerService.GetGas();
            await powerService.GetElec();
            await githubService.GetCommits();
            await githubService.GetGitHubFollowers();
            await githubService.GetGitHubFollowing();
            await githubService.GetGitHubRepo();
            await githubService.GetGitHubStars();
            await devToService.GetDevTo();
        }
    }
}
