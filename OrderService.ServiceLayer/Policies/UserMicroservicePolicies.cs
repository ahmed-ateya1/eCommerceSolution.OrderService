using Microsoft.Extensions.Logging;
using Polly;

namespace OrderService.BusinessLayer.Policies
{
    public class UserMicroservicePolicies : IUserMicroservicePolicies
    {
        private readonly ILogger<UserMicroservicePolicies> _logger;
        private readonly IPollyPolicies _pollyPolicies;

        public UserMicroservicePolicies(ILogger<UserMicroservicePolicies> logger, IPollyPolicies polyPolicies)
        {
            _logger = logger;
            _pollyPolicies = polyPolicies;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCombinePolicy()
        {
            var retryPolicy = _pollyPolicies.GetRetryPolicy(5);
            var circuitBreakerPolicy  = _pollyPolicies.GetCircuitBreakerPolicy(3,TimeSpan.FromMinutes(2));
            var timeoutPolicy = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(10));

            return Policy.WrapAsync<HttpResponseMessage>(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
        }
    }
}
