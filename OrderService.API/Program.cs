using FluentValidation.AspNetCore;
using OrderService.BusinessLayer;
using OrderService.BusinessLayer.HttpClients;
using OrderService.DataAccessLayer;
namespace OrderService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddServiceLayer();
            builder.Services.AddRepositoryLayer(builder.Configuration);
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpClient<UserMicroservicesClient>(client =>
            {
                client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}:" +
                    $"{builder.Configuration["UsersMicroservicePort"]}");
            });
            builder.Services.AddHttpClient<ProductMicroservicesClient>(client =>
            {
                client.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceName"]}:" +
                    $"{builder.Configuration["ProductsMicroservicePort"]}");
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
