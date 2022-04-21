namespace BidderRegistration.Domain
{
    public class BidderData
    {
        public string Id { get; set; }
        public string PartitionKey { get; set; }
        
        public int? MarketplaceCode { get; set; }
        
        public long? AuctionId { get; set; }
        
        public int? AuctionHouseId { get; set; }
        public string CustomerId { get; set; }
        public string BidderId { get; set; }
        public string BidderRef { get; set; }
        public Status? Status { get; set; }
        public string? Cta { get; set; }
    }
}