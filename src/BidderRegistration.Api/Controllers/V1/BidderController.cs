using System.Threading.Tasks;
using BidderRegistration.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BidderRegistration.Api.Contracts;
using BidderRegistration.Api.Mappers;
using FluentValidation;

namespace BidderRegistration.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v1")]
    public partial class BidderController : ControllerBase
    {
        private readonly IBidderRepository _bidderRepository;
        private readonly IValidator<BidderRequest> _upsertBidderValidator;

        public BidderController(IBidderRepository bidderRepository, IValidator<BidderRequest> upsertBidderValidator)
        {
            _upsertBidderValidator = upsertBidderValidator;
            _bidderRepository = bidderRepository;
        }

        /// <summary> Put bidder</summary>
        /// <param name="bidderRequest">The bidder request details</param>
        [HttpPut]
        [Route("bidder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailedResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PutBidder([FromBody] BidderRequest bidderRequest)
        {
            var validateResult = _upsertBidderValidator.ValidateAsync(bidderRequest); 
            if (validateResult.Result.IsValid)
            {
                BidderData bidderData = BidderDataMapper.MapToBidderData(bidderRequest);
                await _bidderRepository.Persist(bidderData);
                return Ok();
            }

            var validationErrors = ValidationFailedResponseMapper.MapToValidationError(validateResult.Result);

            return BadRequest(new ValidationFailedResponse(validationErrors));
        }
    }
}
