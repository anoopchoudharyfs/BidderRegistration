using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace BidderRegistration.Api.UnitTests
{
    using FluentValidation;
    using FluentValidation.TestHelper;
    using global::BidderRegistration.Api.Contracts;
    using global::BidderRegistration.Api.Validators;
    using global::BidderRegistration.Domain;
    using Xunit;

    namespace SLB.CoreBroker.Api.UnitTests.Tests.FluentValidations
    {
        public class ValidatorTests
        {
            private readonly IValidator<BidderRequest> _upsertBidderValidator;

            public ValidatorTests()
            {
                _upsertBidderValidator = new UpsertBidderValidator();
            }          

            [Fact]
            public void CustomerId_When_Characters_GreaterThanFifty_ShouldHave_ValidationError()
            {
                string customerId = new string('a', 51);
                var upsertBidderRequest = CommonUtilities.BidderRequestData(customerId: customerId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_CUSTOMERID.ToString());
            }

            [Fact]
            public void CustomerId_When_Empty_ShouldHave_ValidationError()
            {
                string customerId = "";
                var upsertBidderRequest = CommonUtilities.BidderRequestData(customerId: customerId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_CUSTOMERID.ToString());
            }

            [Theory]
            [InlineData("testcustomer#1")]
            [InlineData("testcustomer/1")]
            [InlineData("testcustomer\\1")]
            [InlineData("testcustomer?1")]
            public void CustomerId__IsInvalid_ShouldHave_ValidationError(string customerId)
            {
                var upsertBidderRequest = CommonUtilities.BidderRequestData(customerId: customerId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_CUSTOMERID.ToString());
            }

            [Fact]
            public void CustomerId__IsValidCharacters_ShouldHave_NoValidationError()
            {
                var customerId = "test!:{}&*()£$%^";
                var upsertBidderRequest = CommonUtilities.BidderRequestData(customerId: customerId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(true);                
            }




            [Fact]  
            public void BidderId_When_Characters_GreaterThanFifty_ShouldHave_ValidationError()
            {
                string bidderId = new string('a', 51);
                var upsertBidderRequest = CommonUtilities.BidderRequestData(bidderId: bidderId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_BIDDERID.ToString());
            }

            [Fact]
            public void BidderId_When_Empty_ShouldHave_ValidationError()
            {
                string bidderId = "";
                var upsertBidderRequest = CommonUtilities.BidderRequestData(bidderId: bidderId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_BIDDERID.ToString());
            }


            [Theory]
            [InlineData("testbidder#1")]
            [InlineData("testbidder/1")]
            [InlineData("testbidder\\1")]
            [InlineData("testbidder?1")]
            public void BidderId_IsInvalid_ShouldHave_ValidationError(string bidderId)
            {
                var upsertBidderRequest = CommonUtilities.BidderRequestData(bidderId: bidderId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_BIDDERID.ToString());
            }

            [Fact]
            public void BidderId_IsValidCharacters_ShouldHave_NoValidationError()
            {
                var bidderId = "test!:{}&*()£$%^";
                var upsertBidderRequest = CommonUtilities.BidderRequestData(bidderId: bidderId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(true);
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void MarketplaceCode_When_LessThanOrEqualZero_ShouldHave_ValidationError(int marketplaceCode)
            {
                var upsertBidderRequest = CommonUtilities.BidderRequestData(MarketplaceCode: marketplaceCode);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_MARKETPLACECODE.ToString());
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void AuctionHouseId_When_LessThanOrEqualZero_ShouldHave_ValidationError(int auctionHouseId)
            {
                var upsertBidderRequest = CommonUtilities.BidderRequestData(AuctionHouseId: auctionHouseId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_AUCTIONHOUSEID.ToString());
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void AuctionId_When_LessThanOrEqualZero_ShouldHave_ValidationError(int globalAuctionId)
            {
                var upsertBidderRequest = CommonUtilities.BidderRequestData(AuctionId: globalAuctionId);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_AUCTIONID.ToString());
            }

            [Fact]
            public void CTA_When_Characters_GreaterThanThousand_ShouldHave_ValidationError()
            {
                string cta = new string('a', 1001);
                var upsertBidderRequest = CommonUtilities.BidderRequestData(cta: cta);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_CTA.ToString());
            }

            [Fact]
            public void BidderRef_When_Characters_GreaterThanFifty_ShouldHave_ValidationError()
            {
                string bidderRef = new string('a', 51);
                var upsertBidderRequest = CommonUtilities.BidderRequestData(bidderRef: bidderRef);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_BIDDERREF.ToString());
            }

            [Fact]
            public void BidderRef_When_Empty_ShouldHave_ValidationError()
            {
                string bidderRef = "";
                var upsertBidderRequest = CommonUtilities.BidderRequestData(bidderRef: bidderRef);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_BIDDERREF.ToString());
            }


            [Theory]
            [InlineData(Status.None)]
            public void STATUS_When_NONE_ShouldHave_ValidationError(Status status)
            {
                var upsertBidderRequest = CommonUtilities.BidderRequestData(status: status);
                var result = _upsertBidderValidator.TestValidate(upsertBidderRequest);
                result.IsValid.Should().Be(false);
                CommonAssertions.AssertResultsContainsError(result, ErrorCode.ERROR_INVALID_STATUS.ToString());
            }
        }

        public static class CommonAssertions
        {
            public static bool AssertResultsContainsError<T>(TestValidationResult<T> result, string error)
            {
                var hasCorrectError = result.Errors.Any(x => x.CustomState.ToString() == error);               
                hasCorrectError.Should().Be(true);
                return hasCorrectError;
            }

            public static bool AssertResultsDoesNotContainError<T>(TestValidationResult<T> result, string error)
            {
                var hasError = result.Errors.Any(x => x.CustomState.ToString() == error);
                hasError.Should().Be(false);
                return hasError;
            }
        }

        public class CommonUtilities
        {
            public static BidderRequest BidderRequestData(
                string customerId = "a_customer_id",
                string bidderId = "a_bidder_id",
                int MarketplaceCode = 201,
                int AuctionHouseId = 20,
                long AuctionId = 30,
                Status status = Status.Approved,
                string bidderRef = "10",
                string cta = "test html")
            {
                return new BidderRequest
                {
                    CustomerId = customerId,
                    BidderId = bidderId,
                    MarketplaceCode = MarketplaceCode,
                    AuctionHouseId = AuctionHouseId,
                    AuctionId = AuctionId,
                    Status = status,
                    BidderRef = bidderRef,
                    Cta = cta
                };
            }
        }

    }
}