using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace BidderRegistration.Api.Application.Policies
{
    public static partial class Polly
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(PollySettings.CircuitBreakerPolicy.ConsecutiveFaultsCount
                    , TimeSpan.FromSeconds(PollySettings.CircuitBreakerPolicy.BreakPeriodInSeconds));
        }
    }
}