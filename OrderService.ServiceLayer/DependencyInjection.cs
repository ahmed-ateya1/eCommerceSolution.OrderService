using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderService.BusinessLayer.HttpClients;
using OrderService.BusinessLayer.MappingProfile;
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
            
            return services;
        }
    }
}
