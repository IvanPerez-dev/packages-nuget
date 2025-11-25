using Microsoft.Extensions.DependencyInjection;

namespace ApiFluentResult.Services
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<List<User>>(new List<User>());

            return services;
        }
    }
}
