using Ocelot.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Values;

namespace ApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            builder.Services
                .AddOcelot()
                .AddPolly();

            var app = builder.Build();

            await app.UseOcelot();
            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}