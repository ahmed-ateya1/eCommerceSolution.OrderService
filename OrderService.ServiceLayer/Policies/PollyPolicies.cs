using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OrderService.BusinessLayer.Policies
{
    public class PollyPolicies : IPollyPolicies
    {
        private readonly ILogger<PollyPolicies> _logger;

        public PollyPolicies(ILogger<PollyPolicies> logger)
        {
            _logger = logger;
        }
        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                retryCount:retryCount,
                sleepDurationProvider: retryAttempt=> TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
                onRetry: (outcome, timespan, retryattempt, context) =>
                {
                    _logger.LogInformation($"Retry {retryattempt} for {context.PolicyKey} at {context.OperationKey}: due to {outcome.Result.StatusCode} time Taken: {timespan}");
                }
                );
            return policy;
        }
        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventCount, TimeSpan dutationOfBreak)
        {
            var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: handledEventCount,
                durationOfBreak: dutationOfBreak,
                onBreak: (outcome, timespan) =>
                {
                    _logger.LogInformation("open Breaker state");
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker is closed");
                }
                );
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
        {
            var policy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy(object model)
        {
            var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .FallbackAsync(async (context) =>
                {
                    _logger.LogInformation($"FallbackPolicy Triggered: the request failed ");
                    var response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.ServiceUnavailable,
                        Content = new StringContent(
                          JsonSerializer.Serialize(model, 
                          new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                            Encoding.UTF8,
                            "application/json")
                    };
                    return await Task.FromResult(response);
                });
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolation(int maxParallelization, int maxQueuingActions)
        {
            var policy = Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization:maxParallelization,
                maxQueuingActions: maxQueuingActions,
                onBulkheadRejectedAsync: (context) =>
                {
                    _logger.LogInformation("Bulkhead Rejected");
                    throw new BulkheadRejectedException("Bulkhead queue is full");
                });
            return policy;
        }
    }
}
