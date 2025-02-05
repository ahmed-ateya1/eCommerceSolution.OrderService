using OrderService.BusinessLayer.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.BusinessLayer.HttpClients
{
    public class UserMicroservicesClient
    {
        private readonly HttpClient _httpClient;

        public UserMicroservicesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UserDto?> GetUserByID(Guid userID)
        {
            var respose = await _httpClient.GetAsync($"/api/Auth/userinfo/{userID}");

            if(!respose.IsSuccessStatusCode)
            {
                if (respose.StatusCode == HttpStatusCode.NotFound) return null;
                else if(respose.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad Request",null,HttpStatusCode.BadRequest);
                }
                else
                {
                    throw new HttpRequestException($"Request is failed and this is status code {respose.StatusCode}");
                }
            }
            return await respose.Content.ReadFromJsonAsync<UserDto>();
        }
    }
}
