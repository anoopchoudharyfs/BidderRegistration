using BidderRegistration.Domain;
using BidderRegistration.Services.Config;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using Xunit;
using Microsoft.ApplicationInsights.Extensibility;

namespace BidderRegistration.API.AdapterTests
{
    public class CosmosDBAdapterTests
    {
        private CosmosDbSettings _cosmosDbSettings = new CosmosDbSettings()
        {
            ContainerName = "bidderdata",
            Database = "bidder-registration",
            ConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
           
        };

        private CosmosClient _cosmosClient;

        [Fact]
        public async Task SaveAndReadFromCosmosDB()
        {
            var repo = new BidderRepository(_cosmosDbSettings, new TelemetryClient(TelemetryConfiguration.CreateDefault()));

            var toCreate = new BidderData()
            {
                Id = "1",
                PartitionKey = "10-4",
                MarketplaceCode = 10,
                AuctionId = 2,
                AuctionHouseId = 3,
                CustomerId = "4",
                BidderId = "5",
                Status = Status.Pending,
                Cta = "This is a test document created by the adapter test.",
            };

            await repo.Persist(toCreate);

            var cosmosClientOptions = new CosmosClientOptions() { SerializerOptions = new CosmosSerializationOptions() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase } };
           
            _cosmosClient = new CosmosClient(_cosmosDbSettings.ConnectionString, cosmosClientOptions);            

            var databaseId = _cosmosDbSettings.Database;

            var container = _cosmosClient.GetContainer(databaseId, _cosmosDbSettings.ContainerName);

            var readResult = await container.ReadItemAsync<BidderData>(toCreate.Id, new PartitionKey(toCreate.PartitionKey));

            Assert.Equal(toCreate.Id, readResult.Resource.Id);
            Assert.Equal(toCreate.PartitionKey, readResult.Resource.PartitionKey);
            Assert.Equal(toCreate.MarketplaceCode, readResult.Resource.MarketplaceCode);
            Assert.Equal(toCreate.AuctionId, readResult.Resource.AuctionId);
            Assert.Equal(toCreate.AuctionHouseId, readResult.Resource.AuctionHouseId);
            Assert.Equal(toCreate.CustomerId, readResult.Resource.CustomerId);
            Assert.Equal(toCreate.BidderId, readResult.Resource.BidderId);
            Assert.Equal(toCreate.Status, readResult.Resource.Status);
            Assert.Equal(toCreate.Cta, readResult.Resource.Cta);

            await container.DeleteItemAsync<BidderData>(toCreate.Id, new PartitionKey(toCreate.PartitionKey));
        }
    }
}
