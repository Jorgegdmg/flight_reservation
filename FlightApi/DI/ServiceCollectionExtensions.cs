using Microsoft.Extensions.DependencyInjection;
using FlightApi.Utils;

namespace FlightApi.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Vinculamos la interfaz con la implementación
            services.AddScoped<IValidaciones, Validaciones>();
            return services;
        }
    }
}
