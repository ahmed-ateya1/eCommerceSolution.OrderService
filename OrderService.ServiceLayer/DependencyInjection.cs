using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderService.BusinessLayer.MappingProfile;
using OrderService.BusinessLayer.Policies;
using OrderService.BusinessLayer.ServiceContracts;
using OrderService.BusinessLayer.Services;
using OrderService.BusinessLayer.Validators;

namespace OrderService.BusinessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(OrderConfig).Assembly);
            services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
            services.AddScoped<IOrderService, OrderServices>();
            services.AddTransient<IProductMicroservicePolicies, ProductMicroservicePolicies>();
            services.AddTransient<IUserMicroservicePolicies, UserMicroservicePolicies>();
            services.AddTransient<IPollyPolicies, PollyPolicies>();

            return services;
        }
    }
}
