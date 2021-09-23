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
        private string APIKEY { get; set; }

        public BlogService(HttpClient httpClient, IConfiguration config)
        {
            Client = httpClient;
            if (!string.IsNullOrEmpty(config.GetValue<string>("BaseURL")))
            {
                Client.BaseAddress = new Uri(config.GetValue<string>("BaseURL"));
            }
            APIKEY = config.GetValue<string>("APIKEY");
        }

        public async Task<List<BlogPosts>> GetBlogsAsync()
        {
            return await Client.GetFromJsonAsync<List<BlogPosts>>(new Uri($"{Client.BaseAddress}api/GetAllBlogs?code={APIKEY}"));
        }

        public async Task<BlogPostsSingle> GetBlogPostAsync(int id)
        {
            return await Client.GetFromJsonAsync<BlogPostsSingle>(new Uri($"{Client.BaseAddress}api/GetPost?id={id}"));
        }

        public async Task<IList<IList<ChartView>>> GetChart(int type, int day, int offSet, string username)
        {
            return await Client.GetFromJsonAsync<IList<IList<ChartView>>>(new Uri($"{Client.BaseAddress}api/GetChart?code={APIKEY}&type={type}&day={day}&offset={offSet}&username={username}"));
        }

        public async Task<List<Metric>> Get(int type)
        {
            return await Client.GetFromJsonAsync<List<Metric>>(new Uri($"{Client.BaseAddress}api/Get?code={APIKEY}&type={type}"));
        }

        public async Task<List<Metric>> GetAll()
        {
            return await Client.GetFromJsonAsync<List<Metric>>(new Uri($"{Client.BaseAddress}api/GetAll?code={APIKEY}"));
        }

        public async Task SaveData(decimal value, int type)
        {
            await Client.PostAsync(new Uri($"{Client.BaseAddress}api/SaveData?code={APIKEY}&type={type}&value={value}"), null);
        }

        public async Task SaveData(decimal value, int type, DateTime To)
        {
            await Client.PostAsync(new Uri($"{Client.BaseAddress}api/SaveData?code={APIKEY}&type={type}&value={value}&date={To}"), null);
        }

        public async Task Delete(int type, DateTime dt)
        {
            await Client.DeleteAsync(new Uri($"{Client.BaseAddress}api/Delete?code={APIKEY}&type={type}&date={dt}"));
        }

        public async Task GetCommits()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetCommits?code={APIKEY}"));
        }

        public async Task GetGitHubStars()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubStars?code={APIKEY}"));
        }

        public async Task GetGitHubRepo()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubRepo?code={APIKEY}"));
        }

        public async Task GetGitHubFollowers()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubFollowers?code={APIKEY}"));
        }

        public async Task GetGitHubFollowing()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGitHubFollowing?code={APIKEY}"));
        }

        public async Task GetTwitterFav()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetTwitterFav?code={APIKEY}"));
        }

        public async Task GetTwitterFollowers()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetTwitterFollowers?code={APIKEY}"));
        }

        public async Task GetTwitterFollowing()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetTwitterFollowing?code={APIKEY}"));
        }

        public async Task GetNumberOfTweets()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetNumberOfTweets?code={APIKEY}"));
        }

        public async Task GetDevTo()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetDevTo?code={APIKEY}"));
        }

        public async Task GetElec()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetElec?code={APIKEY}"));
        }

        public async Task GetGas()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGas?code={APIKEY}"));
        }
    }
}
