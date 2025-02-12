using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderService.BusinessLayer.Dtos;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.BusinessLayer.HttpClients
{
    public class ProductMicroservicesClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductMicroservicesClient> _logger;
        private readonly IDistributedCache _distributedCache;

        public ProductMicroservicesClient(HttpClient httpClient, ILogger<ProductMicroservicesClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<ProductDto?> GetProductByID(Guid productID)
        {
            try
            {
                string cacheKey = $"product:{productID}";
                var cachedProduct = await _distributedCache.GetStringAsync(cacheKey);
                if(cachedProduct != null)
                {
                    return JsonSerializer.Deserialize<ProductDto>(cachedProduct);
                }
                var response = await _httpClient.GetAsync($"/gateway/products/search/productid/{productID}");
                if (!response.IsSuccessStatusCode)
                {
                    if(response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        return await response.Content.ReadFromJsonAsync<ProductDto>();
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException("invalid response", null, response.StatusCode);
                    }
                    else
                    {
                        return new ProductDto
                        {
                            ProductID = Guid.Empty,
                            Category = "Temporarily Unavailable",
                            ProductName = "Temporarily Unavailable",
                            QuantityInStock = 0,
                            UnitPrice = 0,
                        };
                        //throw new HttpRequestException("internal server error", null, response.StatusCode);
                    }
                }
                var product = await response.Content.ReadFromJsonAsync<ProductDto>();
                var productJson= JsonSerializer.Serialize(product);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(1),
                };
                await _distributedCache.SetStringAsync(cacheKey, productJson,options);
                return product;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Request failed because of circuit breaker in open state.");
                return new ProductDto
                {
                    ProductID = Guid.Empty,
                    Category = "Temporarily Unavailable (Broken)",
                    ProductName = "Temporarily Unavailable (Broken)",
                    QuantityInStock = 0,
                    UnitPrice = 0,
                };
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "Request failed due to timeout.");
                return new ProductDto
                {
                    ProductID = Guid.Empty,
                    Category = "Temporarily Unavailable (timeout)",
                    ProductName = "Temporarily Unavailable (timeout)",
                    QuantityInStock = 0,
                    UnitPrice = 0,
                };
            }
            catch (BulkheadRejectedException ex)
            {
                _logger.LogError(ex, "Request failed due to Bulkhead.");
                return new ProductDto
                {
                    ProductID = Guid.Empty,
                    Category = "Temporarily Unavailable (Bulkhead)",
                    ProductName = "Temporarily Unavailable (Bulkhead)",
                    QuantityInStock = 0,
                    UnitPrice = 0,
                };
            }
        }
    }
}