using Microsoft.Extensions.Logging;
using OrderService.BusinessLayer.Dtos;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace OrderService.BusinessLayer.Policies
{
    public class ProductMicroservicePolicies : IProductMicroservicePolicies
    {
        private readonly ILogger<ProductMicroservicePolicies> _logger;
        private readonly IPollyPolicies _pollyPolicies;

        public ProductMicroservicePolicies(ILogger<ProductMicroservicePolicies> logger, IPollyPolicies pollyPolicies)
        {
            _logger = logger;
            _pollyPolicies = pollyPolicies;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCombinePolicy()
        {
            var fallbackPolicy = _pollyPolicies.GetFallbackPolicy(new ProductDto()
            {
                Category = "Temporay unavailable {Fallback}",
                ProductID = Guid.Empty,
                ProductName = "Temporay unavailable {Fallback}",
                QuantityInStock = 0 ,
                UnitPrice = 0
            });
            var bulkheadPolicy = _pollyPolicies.GetBulkheadIsolation(3,50);
            return Policy.WrapAsync<HttpResponseMessage>(bulkheadPolicy, fallbackPolicy);

        }
    }
}
