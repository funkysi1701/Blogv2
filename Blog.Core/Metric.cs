using System;

namespace Blog.Core
{
    public class Metric
    {
        public string Id { get; set; }
        public string PartitionKey { get; set; }
        public decimal? Value { get; set; }
        public DateTime? Date { get; set; }
        public int? Type { get; set; }
    }
}
