using System.ComponentModel.DataAnnotations;
using BidderRegistration.Domain;

namespace BidderRegistration.Api.Contracts
{
    public class BidderRequest
    {
        [Required]
        public int? MarketplaceCode { get; set; }
        [Required]
        public long? AuctionId { get; set; }
        [Required]
        public int? AuctionHouseId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string BidderId { get; set; }

        [Required]
        public Status? Status { get; set; }
        [Required]
        public string BidderRef { get; set; }
        public string? Cta { get; set; }
    }
}