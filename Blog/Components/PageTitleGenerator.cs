using System;

namespace Blog.Components
{
    public static class PageTitleGenerator
    {
        public static string Create(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

            title = title.Replace('-', ' ');
            if (title.Contains("posts"))
            {
                title = title.Replace("posts", "");
                title = title[0..^4];
            }

            string pageTitle = title switch
            {
                "/" => string.Empty,
                _ => $"{title}",
            };
            return pageTitle + " - Funky Si's Blog";
        }
    }
}
