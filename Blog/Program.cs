using Blog.Data.Services;
using ImpSoft.OctopusEnergy.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blog
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<BlogService>();

            builder.Services.AddScoped<DevToService>();
            builder.Services.AddScoped<TwitterService>();
            //builder.Services.AddScoped<PowerService>();
            //builder.Services.AddHttpClient<IOctopusEnergyClient, OctopusEnergyClient>()
            //    .ConfigurePrimaryHttpMessageHandler(h => new HttpClientHandler
            //    {
            //        AutomaticDecompression = System.Net.DecompressionMethods.All
            //    });
            builder.Services.AddSingleton<AppVersionInfo>();
            await builder.Build().RunAsync();
        }
    }
}
