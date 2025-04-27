using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Application.Mappings;
using OnlineStore.Application.Services;
using OnlineStore.Application.Services.Interfaces;
using System.Reflection;

namespace OnlineStore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();

            return services;
        }
    }
}