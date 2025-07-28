using DataAccess.Context;
using Shared.Application.Common.Caching;
using Shared.Application.Common.Interfaces;
using Shared.Infrastructure;
using Shared.Infrastructure.RepositoryQueryFilters;
using Shared.Models;
using System.Reflection;

namespace RumarApi.Infrastructure
{
    internal static class Startup
    {
        internal static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddLocalization()
            .AddServices(typeof(ITransientService), ServiceLifetime.Transient)
            .AddServices(typeof(IScopedService), ServiceLifetime.Scoped)
            .AddRepositories()
            .AddRepositoryQueryAggregator()
            .AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

        internal static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
        {
            var interfaceTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => interfaceType.IsAssignableFrom(t)
                                && t.IsClass && !t.IsAbstract)
                    .Select(t => new
                    {
                        Service = t.GetInterfaces().FirstOrDefault(),
                        Implementation = t
                    })
                    .Where(t => t.Service is not null
                                && interfaceType.IsAssignableFrom(t.Service));

            foreach (var type in interfaceTypes)
            {
                services.AddService(type.Service!, type.Implementation, lifetime);
            }

            return services;
        }

        internal static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime) =>
            lifetime switch
            {
                ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
                ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
                _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
            };

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRumarRepository<>), typeof(ErpRepository<>));

            foreach (var entityType in
                typeof(IEntity).Assembly.GetExportedTypes()
                    .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass)
                    .ToList())
            {
                services.AddScoped(
                    typeof(IRumarRepository<>).MakeGenericType(entityType),
                    sp => Activator.CreateInstance(
                        typeof(ErpRepository<>).MakeGenericType(entityType),
                        sp.GetRequiredService(typeof(ApplicationDbContext)),
                        sp.GetRequiredService(typeof(RumarRepositoryQueryAggregator)))
                    ?? throw new InvalidOperationException($"Couldn't create ErpRepository for aggregateRootType {entityType.Name}"));
            }

            return services;
        }

        public static IServiceCollection AddRepositoryQueryAggregator(this IServiceCollection services)
        {
            // Add ErpRepositoryQueryAggregator
            services.AddSingleton(typeof(RumarRepositoryQueryAggregator), sp => new RumarRepositoryQueryAggregator(sp));
            services.AddScoped<IHeaderService, HeaderService>();

            foreach (var repositoryFilter in
                typeof(IRumarRepositoryQueryFilter<>).Assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRumarRepositoryQueryFilter<>)))
                    .ToList())
            {
                var ifaceGeneric = repositoryFilter.GetInterfaces().FirstOrDefault(i => i.IsGenericType);
                var iface = ifaceGeneric.GenericTypeArguments.FirstOrDefault();

                var implementedInterface = typeof(IRumarRepositoryQueryFilter<>).MakeGenericType(iface);
                var serviceDescriptor = new ServiceDescriptor(implementedInterface, repositoryFilter, ServiceLifetime.Transient);
                services.Add(serviceDescriptor);
            }

            return services;
        }

        public static IServiceCollection AddCacheServices(this IServiceCollection services)
        {
            foreach (var cacheService in
                typeof(Startup).Assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                                t.GetInterfaces().Any(i => i == typeof(ICacheService)))
                    .ToList())
            {
                var iface = cacheService.GetInterfaces().FirstOrDefault();

                var serviceDescriptor = new ServiceDescriptor(iface, cacheService, ServiceLifetime.Scoped);
                services.Add(serviceDescriptor);
            }

            return services;
        }
    }
}
