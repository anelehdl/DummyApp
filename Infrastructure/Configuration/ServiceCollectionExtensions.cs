using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DummyApp.Infrastructure.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind MongoDBSettings from appsettings.json
            services.Configure<MongoDBSettings>(
                configuration.GetSection("MongoDBSettings"));

            // Register MongoDBContext as singleton
            services.AddSingleton<MongoDBContext>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoDBContext(settings);
            });

            return services;
        }
    }
}