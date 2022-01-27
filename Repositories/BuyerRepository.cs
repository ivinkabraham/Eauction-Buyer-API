using Eauction_Buyer_API.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eauction_Buyer_API.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private Container container, productContainer;
        public BuyerRepository(CosmosClient client, string databaseName, string containerName)
        {
            container = client.GetContainer(databaseName, containerName);
            productContainer = client.GetContainer(databaseName, "eauction-seller-collection");
        }

        public async Task<List<BuyerInfo>> GetAllBuyers()
        {
            IQueryable<BuyerInfo> queryable = container.GetItemLinqQueryable<BuyerInfo>(true);
            return await Task.FromResult(queryable.ToList());
        }

        public async Task PlaceBid(BuyerInfo buyer)
        {
            await container.CreateItemAsync(buyer, new PartitionKey(buyer.Id));
        }

        public async Task UpdateBid(BuyerInfo buyer)
        {
            await container.UpsertItemAsync(buyer, new PartitionKey(buyer.Id));
        }

        public async Task<ProductDetails> GetProductById(string productId)
        {
            IQueryable<ProductDetails> queryable = productContainer.GetItemLinqQueryable<ProductDetails>(true);
            queryable = queryable.Where(item => item.Id == productId);
            return await Task.FromResult(queryable.ToList().FirstOrDefault());
        }
    }
}
