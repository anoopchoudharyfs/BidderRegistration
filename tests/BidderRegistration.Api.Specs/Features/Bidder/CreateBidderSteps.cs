using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BidderRegistration.Api.Contracts;
using BidderRegistration.Api.Specs.MockConfiguration;
using BidderRegistration.Api.Validators;
using BidderRegistration.Domain;
using BidderRegistration.Specs.Features;
using BidderRegistration.Specs.MockConfiguration;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Infrastructure;

namespace BidderRegistration.Api.Specs.Features.Bidder
{
    [Binding]
    public class CreateBidderSteps
    {
        private readonly CustomWebApplicationFactory<MockStartup> _factory;
        private readonly ISpecFlowOutputHelper _outputHelper;
        private ScenarioContext _scenarioContext;
        private Mock<IBidderRepository> _bidderRepository;
        private string RequestBody { get { return _scenarioContext["RequestBody"].ToString(); } set { _scenarioContext["RequestBody"] = value; } }
        
        public CreateBidderSteps(CustomWebApplicationFactory<MockStartup> factory, ISpecFlowOutputHelper outputHelper, ScenarioContext scenarioContext)
        {
            _factory = factory;
            _scenarioContext = scenarioContext;
            _bidderRepository = new Mock<IBidderRepository>();
            _bidderRepository.Setup(x => x.Persist(It.IsAny<BidderData>()));            
        }

        [When(@"you send a put bidder request to (.*)")] 
        public async Task WhenYouSendACreateBidderRequestT(string url)
        {
            var httpClient = GetClient(_factory);
            _scenarioContext["Response"] = await httpClient.PutAsync(url, new StringContent(RequestBody, System.Text.Encoding.UTF8, "application/json"));
        }

        [Then(@"the bidder should be persisted in the database as")]
        public void ThenTheBidderIsPersistedInTheDatabase(Table table)
        {
            var bidderDataExpected = table.CreateInstance<BidderData>();
            _bidderRepository.Verify(   x => x.Persist( It.Is<BidderData>(b => CheckBidderData(b, bidderDataExpected))), Times.Once    );
        }

        private static bool CheckBidderData(BidderData x, BidderData bidderDataExpected)
        {
            x.Id.Should().Be(bidderDataExpected.Id);
            x.PartitionKey.Should().Be(bidderDataExpected.PartitionKey);
            x.CustomerId.Should().Be(bidderDataExpected.CustomerId);
            x.BidderId.Should().Be(bidderDataExpected.BidderId);
            x.MarketplaceCode.Should().Be(bidderDataExpected.MarketplaceCode);
            x.AuctionHouseId.Should().Be(bidderDataExpected.AuctionHouseId);
            x.AuctionId.Should().Be(bidderDataExpected.AuctionId);
            x.Status.Should().Be(bidderDataExpected.Status);
            x.Cta.Should().Be(bidderDataExpected.Cta);
            
            return true;
        }

        private HttpClient GetClient(CustomWebApplicationFactory<MockStartup> factory)
        {
            var specsHttpClientFactory = new SpecsHttpClientFactory();
            specsHttpClientFactory.AddStub(_bidderRepository.Object);
            return specsHttpClientFactory.GetClient(factory, _scenarioContext);
        }
    }
}
