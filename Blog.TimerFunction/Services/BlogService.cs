using Blog.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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

        public async Task GetBlogCount()
        {
            var url = Configuration.GetValue<string>("RSSFeed");

            var count = XDocument
                .Load(url)
                .XPathSelectElements("//item")
                .Count();
            await Chart.SaveData(count, (int)MetricType.Blog, Configuration.GetValue<string>("Username1"));
        }
    }
}
