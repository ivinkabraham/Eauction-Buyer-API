using Eauction_Buyer_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eauction_Buyer_API.DataAccess
{
    public interface ICosmosBuyerService
    {
       // Task Publish();

        Task PlaceBid(BuyerInfo buyer);

        Task UpdateBid(string productId, string email, double amount);
    }
}
