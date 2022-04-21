using System.Collections.Generic;

namespace BidderRegistration.Domain.Exceptions
{
    public class ModelBindingValidationError
    {
        public IList<ValidationError> ValidationResults { get; set; }
    }
}
