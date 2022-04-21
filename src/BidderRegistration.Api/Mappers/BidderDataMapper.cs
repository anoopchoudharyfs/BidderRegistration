using BidderRegistration.Domain;
using BidderRegistration.Api.Contracts;

namespace BidderRegistration.Api.Mappers
{  
     public static class BidderDataMapper
     {
        public static BidderData MapToBidderData(BidderRequest bidderRequest)
        {
            return new BidderData
            {
                Id = $"{bidderRequest.AuctionId}-{bidderRequest.CustomerId}-{bidderRequest.MarketplaceCode}",
                PartitionKey = $"{bidderRequest.CustomerId}-{bidderRequest.MarketplaceCode}",
                CustomerId = bidderRequest.CustomerId,
                AuctionHouseId = bidderRequest.AuctionHouseId,
                AuctionId = bidderRequest.AuctionId,
                MarketplaceCode = bidderRequest.MarketplaceCode,
                BidderId = bidderRequest.BidderId,
                BidderRef = bidderRequest.BidderRef,
                Status = bidderRequest.Status,
                Cta = bidderRequest.Cta
            };
        }
     }   
}
