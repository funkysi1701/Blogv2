﻿using Blog.Sitemap.Data.Services;
using System.Text;

namespace Blog.Sitemap
{
    public class SitemapMiddleware
    {
        private readonly BlogService _blogService;

        public SitemapMiddleware(RequestDelegate next, BlogService BlogService)
        {
            _blogService = BlogService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value != null && context.Request.Path.Value.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase))
            {
                var stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/xml";
                string sitemapContent = "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";

                var blogs = await _blogService.GetBlogsAsync(100);
                StringBuilder bld = new();
                foreach (var blog in blogs.Where(x => x.Published))
                {
                    bld.Append("<url>");
                    bld.Append(string.Format("<loc>{0}</loc>", blog.Canonical_Url));
                    if (blog?.Published_At != null)
                    {
                        bld.Append(string.Format("<lastmod>{0}</lastmod>", blog.Published_At.Value.ToString("yyyy-MM-dd")));
                    }

                    bld.Append("</url>");
                }
                sitemapContent += bld.ToString();
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/about");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/pwned-pass");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/config");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "<url>";
                sitemapContent += string.Format("<loc>{0}</loc>", "https://www.funkysi1701.com/metrics");
                sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                sitemapContent += "</url>";
                sitemapContent += "</urlset>";
                using var memoryStream = new MemoryStream();
                var bytes = Encoding.UTF8.GetBytes(sitemapContent);
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(stream, bytes.Length);
            }
        }
    }
}
