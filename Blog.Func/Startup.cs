using Blog.Func;
using Blog.Func.Services;
using ImpSoft.OctopusEnergy.Api;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;

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
                CosmosClientBuilder cosmosClientBuilder = new(config.GetValue<string>("CosmosDBString"));

                return cosmosClientBuilder.WithConnectionModeDirect()
                    .WithApplicationRegion("UK South")
                    .Build();
            });
            builder.Services.AddScoped<GithubService>();
            builder.Services.AddScoped<TwitterService>();
            builder.Services.AddScoped<TwitterServiceV2>();
            builder.Services.AddScoped<DevToService>();
            builder.Services.AddScoped<PowerService>();
            builder.Services.AddHttpClient<IOctopusEnergyClient, OctopusEnergyClient>()
                .ConfigurePrimaryHttpMessageHandler(h => new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All
                });
        }
    }
}
