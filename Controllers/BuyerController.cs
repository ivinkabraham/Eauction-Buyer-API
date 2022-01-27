using Eauction_Buyer_API.DataAccess;
using Eauction_Buyer_API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eauction_Buyer_API.Controllers
{
    [Route("e-auction/api/v1/[controller]")]
    [ApiController]
    public class BuyerController: ControllerBase
    {
        private readonly ICosmosBuyerService _buyerService;
        public BuyerController(ICosmosBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    await _buyerService.Publish();
        //    return Ok();

        //}

        [HttpPost]
        [Route("place-bid")]
        public async Task<IActionResult> Post([FromBody] BuyerInfo buyer)
        {
            await _buyerService.PlaceBid(buyer);
            return Created("place-bid", buyer);
        }

        [HttpPut]
        [Route("update-bid/{productId}/{buyerEmailld}/{newBidAmount}")]
        public async Task Put(string productId, string buyerEmailld, double newBidAmount)
        {
            await _buyerService.UpdateBid(productId, buyerEmailld, newBidAmount);
        }
    }
}
