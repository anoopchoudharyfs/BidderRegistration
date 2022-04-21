﻿using System;
using System.Collections.Generic;
using System.Net.Http;

using BidderRegistration.Api;
using BidderRegistration.Api.Specs.MockConfiguration;
using BidderRegistration.CorrelationId;
using BidderRegistration.Domain;
using BidderRegistration.Specs.MockConfiguration;
using BidderRegistration.Specs.Steps;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TechTalk.SpecFlow;

namespace BidderRegistration.Specs.Features;

public class SpecsHttpClientFactory
{
    public SpecsHttpClientFactory()
    {
    }

    private List<Action<IServiceCollection>> StubActions { get; set; } = new();
    public void AddStub<T>(T stubToAdd) where T : class
    {
        StubActions.Add(x => x.AddSingleton(stubToAdd));
    }

    public HttpClient GetClient(CustomWebApplicationFactory<MockStartup> factory, ScenarioContext scenarioContext)
    {
        var httpClient = InstantiateClient(factory, scenarioContext);

        if (scenarioContext.ContainsKey("Headers"))
        {
            var headers = scenarioContext["Headers"] as List<Header>;

            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }


        return httpClient;
    }

    private HttpClient InstantiateClient(CustomWebApplicationFactory<MockStartup> factory, ScenarioContext scenarioContext)
    {
        var factoryWebHostBuilder = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSolutionRelativeContentRoot("src/BidderRegistration.Api");

            builder.ConfigureTestServices(services =>
            {
                services.AddMvc()
                    .AddApplicationPart(typeof(Startup).Assembly);

                foreach (var stubAction in StubActions)
                {
                    stubAction(services);
                }

                if (scenarioContext.ContainsKey("CorrelationId"))
                    services.AddSingleton(((Mock<ICorrelationIdProvider>)scenarioContext["CorrelationId"]).Object);

                if (scenarioContext.ContainsKey("MockClock"))
                    services.AddSingleton(((Mock<Clock>)scenarioContext["MockClock"]).Object);
            });
        });
        return factoryWebHostBuilder.CreateClient();
    }
}