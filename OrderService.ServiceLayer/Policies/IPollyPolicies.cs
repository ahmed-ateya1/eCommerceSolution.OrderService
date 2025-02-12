using Polly;

namespace OrderService.BusinessLayer.Policies
{
    public interface IPollyPolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount);
        IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventCount , TimeSpan dutationOfBreak);
        IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout);
        IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy(object model);
        IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolation(int maxParallelization , int maxQueuingActions);
    }
}
