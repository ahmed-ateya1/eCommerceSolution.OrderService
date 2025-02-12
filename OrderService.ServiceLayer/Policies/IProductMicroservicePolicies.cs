using Polly;

namespace OrderService.BusinessLayer.Policies
{
    public interface IProductMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetCombinePolicy();
    }
}
