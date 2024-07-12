using BlazorCosmosApp.Components.Data;

namespace AzureCosmosBlazorApp.Data
{
    public interface IEngineerService
    {
        Task DeleteEngineer(string id, string partitionKey);
        Task<List<EngineerModel>> GetAllEngineers();
        Task<EngineerModel> GetEngineerById(string id, string partitionKey);
        Task UpsertEngineer(EngineerModel engineer);
    }
}