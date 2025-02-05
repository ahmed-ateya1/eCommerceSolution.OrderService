using OrderService.BusinessLayer.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.BusinessLayer.HttpClients
{
    public class ProductMicroservicesClient
    {
        private readonly HttpClient _httpClient;

        public ProductMicroservicesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ProductDto?> GetProductByID(Guid productID)
        {
            var response = await _httpClient.GetAsync($"/api/products/search/productid/{productID}");
            if(!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("invalid response",null,response.StatusCode);
                }
                else
                {
                    throw new HttpRequestException("internal server error", null, response.StatusCode);
                }
            }
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
    }
}
