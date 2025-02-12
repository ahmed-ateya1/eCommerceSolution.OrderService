using Polly;

namespace OrderService.BusinessLayer.Policies
{
    public interface IUserMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetCombinePolicy();
    }
}
