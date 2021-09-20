using Blog.Core;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blog.Data.Services
{
    public class BlogService
    {
        private HttpClient Client { get; set; }

        public BlogService(HttpClient httpClient, IConfiguration config)
        {
            Client = httpClient;
            var builder = WebAssemblyHostBuilder.CreateDefault();
            if (!string.IsNullOrEmpty(config.GetValue<string>("BaseURL")))
            {
                Client.BaseAddress = new Uri(config.GetValue<string>("BaseURL"));
            }
            else
            {
                Client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            }
        }

        public async Task<List<BlogPosts>> GetBlogsAsync()
        {
            return await Client.GetFromJsonAsync<List<BlogPosts>>(new Uri($"{Client.BaseAddress}api/GetAllBlogs"));
        }

        public async Task<BlogPostsSingle> GetBlogPostAsync(int id)
        {
            return await Client.GetFromJsonAsync<BlogPostsSingle>(new Uri($"{Client.BaseAddress}api/GetPost?id={id}"));
        }

        public async Task<IList<IList<ChartView>>> GetChart(int type, int day, int OffSet)
        {
            return await Client.GetFromJsonAsync<IList<IList<ChartView>>>(new Uri($"{Client.BaseAddress}api/GetChart?type={type}&day={day}&offset={OffSet}"));
        }
    }
}
