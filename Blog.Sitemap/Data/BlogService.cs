﻿using Blog.Core;

namespace Blog.Sitemap.Data.Services
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

        public async Task<List<BlogPosts>> GetBlogsAsync(int n)
        {
            var res =  await Client.GetFromJsonAsync<List<BlogPosts>>(new Uri($"{Client.BaseAddress}api/GetAllBlogs?n={n}"));
            return res ?? new List<BlogPosts>();
        }

        public async Task<BlogPostsSingle> GetBlogPostAsync(int id)
        {
            var res = await Client.GetFromJsonAsync<BlogPostsSingle>(new Uri($"{Client.BaseAddress}api/GetPost?id={id}"));
            return res ?? new BlogPostsSingle();
        }

        public async Task<IList<IList<ChartView>>> GetChart(int type, int day, int offSet, string username)
        {
            var res = await Client.GetFromJsonAsync<IList<IList<ChartView>>>(new Uri($"{Client.BaseAddress}api/GetChart?type={type}&day={day}&offset={offSet}&username={username}"));
            return res ?? new List<IList<ChartView>>();
        }

        public async Task<List<Metric>> Get(int type)
        {
            var res = await Client.GetFromJsonAsync<List<Metric>>(new Uri($"{Client.BaseAddress}api/Get?type={type}"));
            return res ?? new List<Metric>();
        }

        public async Task<List<Metric>> GetAll()
        {
            var res = await Client.GetFromJsonAsync<List<Metric>>(new Uri($"{Client.BaseAddress}api/GetAll"));
            return res ?? new List<Metric>();
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

        public async Task GetDevTo()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetDevTo"));
        }

        public async Task GetElec()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetElec"));
        }

        public async Task GetGas()
        {
            await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetGas"));
        }
    }
}