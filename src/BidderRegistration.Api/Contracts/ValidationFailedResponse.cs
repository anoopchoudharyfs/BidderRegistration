using System.Collections.Generic;
using BidderRegistration.Domain.Exceptions;

namespace BidderRegistration.Api.Controllers.V1
{
    public class ValidationFailedResponse
    {
        public List<ValidationError> ValidationResults { get; }

        public ValidationFailedResponse(List<ValidationError> validationResults)
        {
            ValidationResults = validationResults;
        }
    }
}
