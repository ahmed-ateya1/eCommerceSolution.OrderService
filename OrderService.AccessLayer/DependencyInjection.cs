﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderService.DataAccessLayer.Repositories;
using OrderService.DataAccessLayer.RepositoryContracts;

namespace OrderService.DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoryLayer(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionStringTemplate = configuration.GetConnectionString("MongoDB")!;
            string connectionString = connectionStringTemplate
              .Replace("$MONGO_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST"))
              .Replace("$MONGO_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT"));

            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            services.AddScoped<IMongoDatabase>(provider =>
            {
                IMongoClient client = provider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_DATABASE"));
            });
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }

    }
}