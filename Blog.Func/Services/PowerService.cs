using ImpSoft.OctopusEnergy.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Func.Services
{
    public class PowerService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }

        private readonly string Key;
        private readonly IOctopusEnergyClient Client;

        public PowerService(IConfiguration configuration, CosmosClient cosmosClient, IOctopusEnergyClient client)
        {
            Configuration = configuration;
            Chart = new Chart(cosmosClient, configuration);
            Client = client;
            Key = Configuration.GetValue<string>("OctopusKey");
        }

        [FunctionName("GetGas")]
        [OpenApiOperation(operationId: "GetGasFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetGasFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetGas();
        }

        public async Task GetGas()
        {
            DateTimeOffset From = new DateTimeOffset(DateTime.UtcNow.AddDays(-30).AddHours(-1).AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            DateTimeOffset To = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            var consumption = await Client.GetGasConsumptionAsync(Key, Configuration.GetValue<string>("OctopusGasMPAN"), Configuration.GetValue<string>("OctopusGasSerial"), From, To, Interval.Hour);
            await CheckConsumption(14, consumption);
        }

        [FunctionName("GetElec")]
        [OpenApiOperation(operationId: "GetElecFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        public async Task GetElecFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await GetElec();
        }

        public async Task GetElec()
        {
            DateTimeOffset From = new DateTimeOffset(DateTime.UtcNow.AddDays(-30).AddHours(-1).AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            DateTimeOffset To = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            var consumption = await Client.GetElectricityConsumptionAsync(Key, Configuration.GetValue<string>("OctopusElecMPAN"), Configuration.GetValue<string>("OctopusElecSerial"), From, To, Interval.Hour);
            await CheckConsumption(15, consumption);
        }

        public async Task CheckConsumption(int Id, IEnumerable<Consumption> consumption)
        {
            var exist = Chart.Get(Id);
            foreach (var item in consumption)
            {
                if (exist.Any(x => x.Date.Value == item.Start.UtcDateTime.Date))
                {
                    await Chart.Delete(Id, item.Start.UtcDateTime);
                }
                await Chart.SaveData(item.Quantity, Id, item.Start.UtcDateTime, Configuration.GetValue<string>("Username1"));
            }
        }
    }
}
