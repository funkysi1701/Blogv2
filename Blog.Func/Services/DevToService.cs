using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Func.Services
{
    public class DevToService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }
        private readonly List<string> users;
        private GetAllBlogs GetAllBlogs { get; set; }

        public DevToService(IConfiguration configuration, CosmosClient cosmosClient)
        {
            Configuration = configuration;
            Chart = new Chart(cosmosClient, configuration);
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1")
            };
            GetAllBlogs = new GetAllBlogs();
        }

        public async Task GetDevTo()
        {
            foreach (var username in users)
            {
                var blogs = await GetAllBlogs.GetAll(Configuration, 200);
                await Chart.SaveData(blogs.Count, 9, username);
                await Chart.SaveData(blogs.Count(x => x.Published), 10, username);
                int views = 0;
                int reactions = 0;
                int comments = 0;
                foreach (var item in blogs)
                {
                    views += item.Page_Views_Count;
                    reactions += item.Positive_Reactions_Count;
                    comments += item.Comments_Count;
                }
                await Chart.SaveData(views, 11, username);
                await Chart.SaveData(reactions, 12, username);
                await Chart.SaveData(comments, 13, username);
            }
        }
    }
}
