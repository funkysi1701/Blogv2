using Blog.Core;
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

        public BlogService(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<List<BlogPosts>> GetBlogsAsync()
        {
            return await Client.GetFromJsonAsync<List<BlogPosts>>(new Uri($"{Client.BaseAddress}/api/GetAllBlogs"));
        }

        public async Task<BlogPostsSingle> GetBlogPostAsync(int id)
        {
            return await Client.GetFromJsonAsync<BlogPostsSingle>(new Uri($"{Client.BaseAddress}/api/GetPost?id={id}"));
        }
    }
}
