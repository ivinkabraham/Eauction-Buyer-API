﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Eauction_Buyer_API.Models
{
    public class BuyerInfo
    {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 5)]
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "pin")]
        public string Pin { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10)]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "bidAmount")]
        public double BidAmount { get; set; }
    }
}
