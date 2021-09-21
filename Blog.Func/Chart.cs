using Blog.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
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

        public Chart(CosmosClient cosmosClient)
        {
            var _cosmosClient = cosmosClient;

            var _database = _cosmosClient.GetDatabase("Metrics");
            _container = _database.GetContainer("Metrics");
        }

        [FunctionName("GetChart")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "day", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **day** parameter")]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **offset** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IList<IList<ChartView>>), Description = "The OK response")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetChart");

            MetricType type = (MetricType)int.Parse(req.Query["type"]);
            MyChartType day = (MyChartType)int.Parse(req.Query["day"]);
            int OffSet = int.Parse(req.Query["offset"]);
            var result = GetChart(type, day, OffSet);
            return new OkObjectResult(result);
        }

        public IList<IList<ChartView>> GetChart(MetricType type, MyChartType day, int OffSet)
        {
            var metrics = _container.GetItemLinqQueryable<Metric>(true).Where(x => x.Type == (int)type).ToList();
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
