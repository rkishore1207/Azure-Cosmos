using BlazorCosmosApp.Components.Data;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosBlazorApp.Data
{
    public class EngineerService : IEngineerService
    {
        public string? AzureCosmosConnectionString { get; set; } = Environment.GetEnvironmentVariable("AZURE_COSMOS_CONNECTION_STRING");
        public string? AzureCosmosDB { get; set; } = Environment.GetEnvironmentVariable("AZURE_COSMOS_DB");
        public string? AzureCosmosContainer { get; set; } = Environment.GetEnvironmentVariable("AZURE_COSMOS_CONTAINER");

        private Container GetClientContainer()
        {
            var cosmosDbClient = new CosmosClient(AzureCosmosConnectionString);
            var container = cosmosDbClient.GetContainer(AzureCosmosDB, AzureCosmosContainer);
            return container;
        }

        public async Task UpsertEngineer(EngineerModel engineer)
        {
            try
            {
                if (engineer.id == Guid.Empty || engineer.id == null)
                {
                    engineer.id = Guid.NewGuid();
                }
                var container = GetClientContainer();
                var response = await container.UpsertItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
                Console.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteEngineer(string? id, string? partitionKey) //Partition key is Optional
        {
            try
            {
                var container = GetClientContainer();
                var response = await container.DeleteItemAsync<EngineerModel>(id, new PartitionKey(partitionKey));
                Console.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<EngineerModel>> GetAllEngineers()
        {
            try
            {
                List<EngineerModel> engineers = new List<EngineerModel>();
                var container = GetClientContainer();
                QueryDefinition definition = new QueryDefinition("SELECT * FROM K");
                FeedIterator<EngineerModel> allEngineers = container.GetItemQueryIterator<EngineerModel>(definition);

                while (allEngineers.HasMoreResults)
                {
                    FeedResponse<EngineerModel> response = await allEngineers.ReadNextAsync();
                    foreach (var item in response)
                    {
                        engineers.Add(item);
                    }
                }
                return engineers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EngineerModel> GetEngineerById(string id, string partitionKey)
        {
            try
            {
                var container = GetClientContainer();
                var response = await container.ReadItemAsync<EngineerModel>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
