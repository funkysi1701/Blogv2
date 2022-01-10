using Blog.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
IConfiguration config = new ConfigurationBuilder()

                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

CosmosClientBuilder cosmosClientBuilderOld = new(config["CosmosDBStringOld"]);
var cosmosClientOld = cosmosClientBuilderOld.Build();
var databaseOld = cosmosClientOld.GetDatabase(config["DatabaseName"]);
var containerOld = databaseOld.GetContainer(config["ContainerName"]);

CosmosClientBuilder cosmosClientBuilderNew = new(config["CosmosDBStringNew"]);
var cosmosClientNew = cosmosClientBuilderNew.Build();
var databaseNew = cosmosClientNew.GetDatabase(config["DatabaseName"]);
var containerNew = databaseNew.GetContainer(config["ContainerName"]);

for (int i = 0; i < (int)MetricType.Electricity + 1; i++)
{
    var m = containerOld.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions { MaxItemCount = -1 }).Where(x => x.Type == i).ToList();
    foreach (var item in m)
    {
        try
        {
            await containerNew.CreateItemAsync<Metric>(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        Console.WriteLine(item.Date?.ToString("yyyy-MM-dd HH:mm"));
    }
}
