using Blog.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blog.Func
{
    public class GetAllBlogs
    {
        [FunctionName("GetAllBlogs")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<BlogPosts>), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("GetAllBlogs");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var posts = await GetAll(config);
            return new OkObjectResult(posts);
        }

        public async Task<List<BlogPosts>> GetAll(IConfiguration config)
        {
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("api-key", config.GetValue<string>("DEVTOAPI"));
            var baseurl = config.GetValue<string>("DEVTOURL");
            using HttpResponseMessage httpResponse = await Client.GetAsync(new Uri($"{baseurl}articles/me/all?per_page=200"));
            string result = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<BlogPosts>>(result);
        }
    }
}
