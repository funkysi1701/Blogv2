using Blog.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using System.Net;
using System.Threading.Tasks;

namespace Blog.Func
{
    public class Chart
    {
        private readonly Container _container;

        public Chart(CosmosClient cosmosClient, IConfiguration Configuration)
        {
            var _cosmosClient = cosmosClient;
            var _database = _cosmosClient.GetDatabase(Configuration.GetValue<string>("DatabaseName"));
            _container = _database.GetContainer(Configuration.GetValue<string>("ContainerName"));
        }

        public async Task<IActionResult> SaveData(decimal value, int type, string Username)
        {
            var m = new Metric
            {
                MetricId = DateTime.UtcNow.Ticks,
                id = DateTime.UtcNow.Ticks.ToString(),
                Date = DateTime.UtcNow,
                Type = type,
                Value = value,
                PartitionKey = "1",
                Username = Username
            };
            await _container.CreateItemAsync(m);
            return new OkResult();
        }

        public async Task<IActionResult> SaveData(decimal value, int type, DateTime To, string Username)
        {
            var m = new Metric
            {
                MetricId = DateTime.UtcNow.Ticks,
                id = DateTime.UtcNow.Ticks.ToString(),
                Date = To,
                Type = type,
                Value = value,
                PartitionKey = "1",
                Username = Username
            };
            await _container.CreateItemAsync(m);
            return new OkResult();
        }

        public async Task Delete(int type, DateTime dt)
        {
            var m = _container.GetItemLinqQueryable<Metric>(true).Where(x => x.Type == type && x.Date == dt).ToList();
            foreach (var item in m)
            {
                await _container.DeleteItemAsync<Metric>(item.id, new PartitionKey(item.PartitionKey));
            }
        }

        [FunctionName("GetChart")]
        [OpenApiOperation(operationId: "GetChart", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "day", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **day** parameter")]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **offset** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IList<IList<ChartView>>), Description = "The OK response")]
        public IActionResult GetChart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetChart");

            MetricType type = (MetricType)int.Parse(req.Query["type"]);
            MyChartType day = (MyChartType)int.Parse(req.Query["day"]);
            int OffSet = int.Parse(req.Query["offset"]);
            string username = req.Query["username"];
            var result = GetChartDetails(type, day, OffSet, username);
            return new OkObjectResult(result);
        }

        [FunctionName("Get")]
        [OpenApiOperation(operationId: "GetFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<Metric>), Description = "The OK response")]
        public List<Metric> GetFn([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            int type = int.Parse(req.Query["type"]);
            return Get(type);
        }

        public List<Metric> Get(int type)
        {
            return _container.GetItemLinqQueryable<Metric>(true).Where(x => x.Type == type).ToList();
        }

        [FunctionName("GetAll")]
        [OpenApiOperation(operationId: "GetAllFn", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<Metric>), Description = "The OK response")]
        public List<Metric> GetAllFn([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return GetAll();
        }

        public List<Metric> GetAll()
        {
            return _container.GetItemLinqQueryable<Metric>(true).ToList();
        }

        public IList<IList<ChartView>> GetChartDetails(MetricType type, MyChartType day, int OffSet, string username)
        {
            var metrics = _container.GetItemLinqQueryable<Metric>(true).Where(x => x.Type == (int)type && x.Username == username).ToList();
            List<Metric> LiveMetrics;
            List<Metric> PrevMetrics;
            if (type >= MetricType.Gas)
            {
                OffSet++;
            }

            if (day == MyChartType.Hourly)
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.AddHours(-24 * (OffSet + 1)) && x.Date <= DateTime.Now.AddHours(-24 * OffSet)).ToList();
                PrevMetrics = metrics.Where(x => x.Date > DateTime.Now.AddHours(-24 * (OffSet + 2)) && x.Date <= DateTime.Now.AddHours(-24 * (OffSet + 1))).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
            else if (day == MyChartType.Daily)
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.AddDays(-14)).ToList();
                PrevMetrics = metrics.Where(x => x.Date <= DateTime.Now.AddDays(-14) && x.Date > DateTime.Now.AddDays(-28)).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
            else
            {
                LiveMetrics = metrics.ToList();
                PrevMetrics = metrics.ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
        }

        private static IList<IList<ChartView>> GetResult(List<Metric> metrics, List<Metric> Prevmetrics)
        {
            var result = new List<ChartView>();
            foreach (var item in metrics.Where(x => x.Date != null))
            {
                var c = new ChartView
                {
                    Date = item.Date.Value,
                    Total = item.Value
                };
                result.Add(c);
            }
            var prevresult = new List<ChartView>();
            foreach (var previtem in Prevmetrics.Where(x => x.Date != null))
            {
                var c = new ChartView
                {
                    Date = previtem.Date.Value,
                    Total = previtem.Value
                };
                prevresult.Add(c);
            }
            var final = new List<IList<ChartView>>
            {
                result,
                prevresult
            };
            return final;
        }
    }
}
