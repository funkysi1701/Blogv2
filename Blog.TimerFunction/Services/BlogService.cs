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
            var content = await DownloadData(url);
            if (content != null)
            {
                await File.WriteAllBytesAsync($"file.xml", content);

                var count = XDocument
                .Load("file.xml")
                .XPathSelectElements("//item")
                .Count();
                await Chart.SaveData(count, (int)MetricType.OldBlog, Configuration.GetValue<string>("Username1"));
            }
        }

        public async Task<byte[]?> DownloadData(string url)
        {
            using (var client = new HttpClient())
            using (var result = await client.GetAsync(url))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Tls13;
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                return result.IsSuccessStatusCode ? await result.Content.ReadAsByteArrayAsync() : null;
            }
        }
    }
}
