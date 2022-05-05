using Blog.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Blog.TimerFunction.Services
{
    public class BlogService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }

        public BlogService(IConfiguration Configuration, CosmosClient cosmosClient)
        {
            this.Configuration = Configuration;
            Chart = new Chart(cosmosClient, Configuration);
        }

        public async Task GetBlogCount(ILogger log)
        {
            var url = Configuration.GetValue<string>("RSSFeed");

            var count = XDocument
                .Load(url)
                .XPathSelectElements("//item")
                .Count();
            await Chart.SaveData(count, (int)MetricType.Blog, Configuration.GetValue<string>("Username1"));
        }

        public async Task GetOldBlogCount(ILogger log)
        {
            var url = Configuration.GetValue<string>("OldRSSFeed");
            var content = await DownloadData(url, log);
            if (content != null)
            {
                await File.WriteAllBytesAsync($"C:\\local\\Temp\\file.xml", content);
                log.LogInformation("File Downloaded");
                var count = XDocument
                .Load("C:\\local\\Temp\\file.xml")
                .XPathSelectElements("//item")
                .Count();
                log.LogInformation($"{count} posts found");
                await Chart.SaveData(count, (int)MetricType.OldBlog, Configuration.GetValue<string>("Username1"));
            }
            else
            {
                log.LogError("Download Failed");
            }
        }

        public static async Task<byte[]> DownloadData(string url, ILogger log)
        {
            try
            {
                using var client = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                using var result = await client.GetAsync(url);
                return result.IsSuccessStatusCode ? await result.Content.ReadAsByteArrayAsync() : null;
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                log.LogError(e.InnerException?.Message);
                log.LogError(e, "Error");
                return null;
            }
        }
    }
}
