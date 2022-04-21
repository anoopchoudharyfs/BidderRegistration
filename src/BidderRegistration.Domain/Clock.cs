using System;

namespace BidderRegistration.Domain
{
    public class Clock
    {
        public virtual DateTime GetCurrentUtcDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}