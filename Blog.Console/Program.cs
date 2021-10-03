using Blog.Core;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
IConfiguration config = new ConfigurationBuilder()

                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(config["CosmosDBString"]);
var cosmosClient = cosmosClientBuilder.Build();
var database = cosmosClient.GetDatabase(config["DatabaseName"]);
var container = database.GetContainer(config["ContainerName"]);

var listofdates = new List<DateTime?>();
var totaln = container.GetItemLinqQueryable<Metric>(true).Where(x => x.Type >= (int)MetricType.Gas).ToList();
for (int i = 1; i < 13; i++)
{
    for (int j = 1; j < 32; j++)
    {
        var n = totaln.Where(x => x.Date.Value.Day == j && x.Date.Value.Month == i).ToList();
        foreach (var item in n.OrderBy(x => x.Date))
        {
            if (!listofdates.Contains(item.Date))
            {
                listofdates.Add(item.Date);
                Console.WriteLine(item.Date?.ToString("yyyy-MM-dd HH:mm"));
                Console.WriteLine(item.Value.ToString());
                Console.WriteLine("###");
            }
            else
            {
                await container.DeleteItemAsync<Metric>(item.id.ToString(), new Microsoft.Azure.Cosmos.PartitionKey(item.PartitionKey));
            }
        }
    }
    Console.WriteLine($"Press any key... {i}");
    Console.ReadKey();
}

for (int i = 0; i < (int)MetricType.Electricity + 1; i++)
{
    var m = container.GetItemLinqQueryable<Metric>(true).Where(x => x.Type == i).ToList();
    foreach (var item in m)
    {
        if (string.IsNullOrEmpty(item.Username))
        {
            Console.WriteLine(item.MetricId.ToString());
            Console.WriteLine(item.id.ToString());
            Console.WriteLine(item.Type.ToString());
            Console.WriteLine(item.Date?.ToString("yyyy-MM-dd HH:mm"));
            Console.WriteLine(item.Value.ToString());
            item.Username = "funkysi1701";
            Console.WriteLine(item.PartitionKey.ToString());
            Console.WriteLine("###");
            await container.ReplaceItemAsync<Metric>(item, item.id.ToString());
        }
    }
}
