using System.Collections.Generic;

namespace BidderRegistration.Domain.Entities
{
    public class Marketplace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlatformId { get; set; }
    }

    public class DestinationPlatform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Marketplace> Marketplaces { get; set; }
    }
    public class MessageDestinationPlatform
    {
        public int PlatformId { get; set; }
        public int[] Marketplaces { get; set; }
    }
}
