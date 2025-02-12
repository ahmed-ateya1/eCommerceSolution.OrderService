using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderService.BusinessLayer.Dtos;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.BusinessLayer.HttpClients
{
    public class UserMicroservicesClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserMicroservicesClient> _logger;
        private readonly IDistributedCache _distributedCache;

        public UserMicroservicesClient(HttpClient httpClient, ILogger<UserMicroservicesClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }
        public async Task<UserDto?> GetUserByID(Guid userID)
        {
            try
            {
                var cacheKey = $"user:{userID}";
                var cachedResult = await _distributedCache.GetStringAsync(cacheKey);
                if(cachedResult != null)
                {
                    return JsonSerializer.Deserialize<UserDto>(cachedResult);
                }
                var respose = await _httpClient.GetAsync($"/gateway/Auth/userinfo/{userID}");

                if (!respose.IsSuccessStatusCode)
                {
                    if (respose.StatusCode == HttpStatusCode.NotFound) return null;
                    else if(respose.StatusCode == HttpStatusCode.ServiceUnavailable)
                        return await respose.Content.ReadFromJsonAsync<UserDto>();
                    else if (respose.StatusCode == HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException("Bad Request", null, HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        return new UserDto()
                        {
                            Email = "Temporarily Unavailable",
                            Gender = "Temporarily Unavailable",
                            PersonName = "Temporarily Unavailable",
                            UserID = Guid.Empty
                        };
                        //throw new HttpRequestException($"Request is failed and this is status code {respose.StatusCode}");
                    }
                }
                var user =  await respose.Content.ReadFromJsonAsync<UserDto>();
                var userJson = JsonSerializer.Serialize(user);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };
                await _distributedCache.SetStringAsync(cacheKey, userJson,options);
                return user;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Request failed because of circuit breaker in open state.");
                return new UserDto()
                {
                    Email = "Temporarily Unavailable (Broken)",
                    Gender = "Temporarily Unavailable (Broken)",
                    PersonName = "Temporarily Unavailable (Broken)",
                    UserID = Guid.Empty
                };
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "Request failed due to timeout.");
                return new UserDto()
                {
                    Email = "Temporarily Unavailable (Broken)",
                    Gender = "Temporarily Unavailable (Broken)",
                    PersonName = "Temporarily Unavailable (Broken)",
                    UserID = Guid.Empty
                };
            }
       

        }
    }
}
