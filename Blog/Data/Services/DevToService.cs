using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data.Services
{
    public class DevToService
    {
        private BlogService BlogService { get; set; }

        public DevToService(BlogService blogService)
        {
            BlogService = blogService;
        }

        public async Task GetDevTo()
        {
            var blogs = await BlogService.GetBlogsAsync();
            await BlogService.SaveData(blogs.Count, 9);
            await BlogService.SaveData(blogs.Count(x => x.Published), 10);
            int views = 0;
            int reactions = 0;
            int comments = 0;
            foreach (var item in blogs)
            {
                views += item.Page_Views_Count;
                reactions += item.Positive_Reactions_Count;
                comments += item.Comments_Count;
            }
            await BlogService.SaveData(views, 11);
            await BlogService.SaveData(reactions, 12);
            await BlogService.SaveData(comments, 13);
        }
    }
}
