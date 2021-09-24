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

        [FunctionName("SaveData1")]
        public async Task Run1([TimerTrigger("0 59 1,13 * * *",RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFav(log);
        }

        [FunctionName("SaveData2")]
        public async Task Run2([TimerTrigger("0 59 2,14 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFollowers(log);
        }

        [FunctionName("SaveData3")]
        public async Task Run3([TimerTrigger("0 59 3,15 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFollowing(log);
        }

        [FunctionName("SaveData4")]
        public async Task Run4([TimerTrigger("0 59 4,16 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetNumberOfTweets(log);
        }

        [FunctionName("SaveData5")]
        public async Task Run5([TimerTrigger("0 59 5,17 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await powerService.GetGas();
        }

        [FunctionName("SaveData6")]
        public async Task Run6([TimerTrigger("0 59 6,18 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await powerService.GetElec();
        }

        [FunctionName("SaveData7")]
        public async Task Run7([TimerTrigger("0 59 7,19 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetCommits();
        }

        [FunctionName("SaveData8")]
        public async Task Run8([TimerTrigger("0 59 8,20 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubFollowers();
        }

        [FunctionName("SaveData9")]
        public async Task Run9([TimerTrigger("0 59 9,21 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubFollowing();
        }

        [FunctionName("SaveData10")]
        public async Task Run10([TimerTrigger("0 59 10,22 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubRepo();
        }

        [FunctionName("SaveData11")]
        public async Task Run11([TimerTrigger("0 59 11,23 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubStars();
        }

        [FunctionName("SaveData12")]
        public async Task Run12([TimerTrigger("0 59 12,0 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await devToService.GetDevTo();
        }
    }
}
