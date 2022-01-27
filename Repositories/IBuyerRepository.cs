using Eauction_Buyer_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eauction_Buyer_API.Repositories
{
    public interface IBuyerRepository
    {
        Task<List<BuyerInfo>> GetAllBuyers();

        Task PlaceBid(BuyerInfo buyer);

        Task UpdateBid(BuyerInfo buyer);

        Task<ProductDetails> GetProductById(string productId);
    }
}
