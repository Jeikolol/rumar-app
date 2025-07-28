using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;

namespace RumarApp.ApiClient
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddApiServices(this IServiceCollection services) =>
            services.AddServices(typeof(IApiService<,>), ServiceLifetime.Transient);

        internal static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
        {
            var interfaceTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == interfaceType)
                        && t.IsClass && !t.IsAbstract)
                    .Select(t => new
                    {
                        Service = t.GetInterfaces().FirstOrDefault(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == interfaceType),
                        Implementation = t
                    })
                    .Where(t => t.Service is not null);

            foreach (var type in interfaceTypes)
            {
                services.AddService(type.Service!, type.Implementation, lifetime);
            }

            return services;
        }

        internal static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            var hasHttpClientCtor = implementationType
                .GetConstructors()
                .Any(ctor => ctor.GetParameters().Any(p => p.ParameterType == typeof(HttpClient)));

            if (hasHttpClientCtor)
            {
                services.AddHttpClient(implementationType.FullName ?? implementationType.Name)
                    .ConfigureHttpClient(_ => { }); // No-op, just creates the named client

                // Register the implementationType using factory
                services.Add(new ServiceDescriptor(implementationType, sp =>
                {
                    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                    var configuration = sp.GetRequiredService<IConfiguration>();

                    var httpClient = httpClientFactory.CreateClient(implementationType.FullName ?? implementationType.Name);

                    // Read ApiSettings from configuration (assumes you have a section called ApiSettings)
                    var baseUrl = configuration.GetSection("ApiSettings:BaseUrl").Value;
                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        httpClient.BaseAddress = new Uri(baseUrl);
                    }

                    return Activator.CreateInstance(implementationType, httpClient)!;
                }, lifetime));

                // Register interface to implementation
                services.Add(new ServiceDescriptor(serviceType, sp =>
                    sp.GetRequiredService(implementationType), lifetime));

                return services;
            }

            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));

            return lifetime switch
            {
                ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
                ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
                _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
            };
        }
    }
}
