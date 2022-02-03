using Eauction_Buyer_API.MessageSharer;
using Eauction_Buyer_API.Models;
using Eauction_Buyer_API.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eauction_Buyer_API.DataAccess
{
    public class CosmosBuyerService: ICosmosBuyerService
    {
        private readonly IBuyerRepository _repository;
        private readonly IRabbitMQCreator _rabbitMqCreator;

        public CosmosBuyerService(IBuyerRepository buyerRepository
            , IRabbitMQCreator rabbitMqCreator
            )
        {
            _repository = buyerRepository;
            _rabbitMqCreator = rabbitMqCreator;
        }

        public async Task PlaceBid(BuyerInfo buyer)
        {
            var productinfo = await _repository.GetProductById(buyer.ProductId);
            if (productinfo.BidEndDate < DateTime.Now)
            {
                throw new ArgumentException("Bid cannot be placed after the bid end date");
            }
            var buyersList = await _repository.GetAllBuyers();
            if (buyersList != null && buyersList.Count > 0)
            {
                bool isBidExists = buyersList.Any(x => x.ProductId == buyer.ProductId && x.Email == buyer.Email);
                if (isBidExists)
                {
                    throw new ArgumentException("More than one bid on a product by the same user is not allowed");
                }
            }

            buyer.Id = Guid.NewGuid().ToString();

            //Add message to RabbitMq
            _rabbitMqCreator.Publish(String.Format("New bid placed by {0} {1}", buyer.FirstName, buyer.LastName));

            await _repository.PlaceBid(buyer);
        }

        public async Task UpdateBid(string productId, string email, double amount)
        {
            var productinfo = await _repository.GetProductById(productId);
            if (productinfo.BidEndDate < DateTime.Now)
            {
                throw new ArgumentException("Bid Amount cannot be updated after the bid end date");
            }

            BuyerInfo buyer = new BuyerInfo();
            var buyersList = await _repository.GetAllBuyers();
            if (buyersList != null && buyersList.Count > 0)
            {
                buyer = buyersList.Where(x => x.ProductId == productId && x.Email == email).FirstOrDefault();
                if (buyer != null)
                {
                    buyer.BidAmount = amount;
                }
            }

            await _repository.UpdateBid(buyer);
        }

        //public async Task Publish()
        //{
        //    var fname = "ivin";
        //    var lname = "abraham";
        //    //Send message to RabbitMq
        //    _rabbitMqCreator.Publish(String.Format("New bid placed by {0} {1}", fname, lname));

        //}
    }
}
