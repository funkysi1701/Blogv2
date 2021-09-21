using Blog.Core;
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
            if (!string.IsNullOrEmpty(config.GetValue<string>("BaseURL")))
            {
                Client.BaseAddress = new Uri(config.GetValue<string>("BaseURL"));
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

        public async Task<IList<IList<ChartView>>> GetChart(int type, int day, int offSet, string username)
        {
            return await Client.GetFromJsonAsync<IList<IList<ChartView>>>(new Uri($"{Client.BaseAddress}api/GetChart?type={type}&day={day}&offset={offSet}&username={username}"));
        }

        public async Task<List<Metric>> Get(int type)
        {
            return await Client.GetFromJsonAsync<List<Metric>>(new Uri($"{Client.BaseAddress}api/Get?type={type}"));
        }

        public async Task<List<Metric>> GetAll()
        {
            return await Client.GetFromJsonAsync<List<Metric>>(new Uri($"{Client.BaseAddress}api/GetAll"));
        }

        public async Task SaveData(decimal value, int type)
        {
            await Client.PostAsync(new Uri($"{Client.BaseAddress}api/SaveData?type={type}&value={value}"), null);
        }

        public async Task SaveData(decimal value, int type, DateTime To)
        {
            await Client.PostAsync(new Uri($"{Client.BaseAddress}api/SaveData?type={type}&value={value}&date={To}"), null);
        }

        public async Task Delete(int type, DateTime dt)
        {
            await Client.DeleteAsync(new Uri($"{Client.BaseAddress}api/Delete?type={type}&date={dt}"));
        }

        public async Task GetCommits()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetCommits"));
        }

        public async Task GetGitHubStars()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubStars"));
        }

        public async Task GetGitHubRepo()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubRepo"));
        }

        public async Task GetGitHubFollowers()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubFollowers"));
        }

        public async Task GetGitHubFollowing()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubFollowing"));
        }

        public async Task GetTwitterFav()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetTwitterFav"));
        }

        public async Task GetTwitterFollowers()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetTwitterFollowers"));
        }

        public async Task GetTwitterFollowing()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetTwitterFollowing"));
        }

        public async Task GetNumberOfTweets()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetNumberOfTweets"));
        }
    }
}
