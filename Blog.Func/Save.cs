﻿using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Blog.Func
{
    public static class Save
    {
        [FunctionName("SaveData")]
        public static void Run([TimerTrigger("0 59 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var client = new HttpClient();
            log.LogInformation($"URL: {config.GetValue<string>("URL")}");
            client.GetAsync(config.GetValue<string>("URL"));
        }
    }
}
