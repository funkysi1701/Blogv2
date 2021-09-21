using Blog.Func;
using Blog.Func.Services;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Blog.Func
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            builder.Services.AddSingleton((s) =>
            {
                CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(config.GetValue<string>("CosmosDBString"));

                return cosmosClientBuilder.WithConnectionModeDirect()
                    .WithApplicationRegion("UK South")
                    .Build();
            });
            builder.Services.AddScoped<GithubService>();
            builder.Services.AddScoped<TwitterService>();
        }
    }
}
