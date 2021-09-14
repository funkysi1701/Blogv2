using Blog.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Blog.Func.context
{
    class MetricsContext : DbContext
    {
        private IConfiguration Config { get; set; }

        public MetricsContext(DbContextOptions<MetricsContext> options) : base(options)
        {
        }

        public MetricsContext(IConfiguration Configuration)
        {
            Config = Configuration;
        }

        public MetricsContext(DbContextOptions<MetricsContext> options, IConfiguration Configuration) : base(options)
        {
            Config = Configuration;
        }

        public virtual DbSet<Metric> Metrics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseCosmos(
                    Config.GetValue<string>("CosmosDBURI"),
                    Config.GetValue<string>("CosmosDBKey"),
                    databaseName: Config.GetValue<string>("CosmosName"));
            }
        }
    }
}
