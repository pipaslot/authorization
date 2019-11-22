#if NETSTANDARD2_0
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissions<TKey, TPermissionStore, TIdentityProvider>(this IServiceCollection services)
            where TIdentityProvider : class, IIdentityProvider<TKey>
            where TPermissionStore : class, IPermissionStore
        {
            return services.AddPermissions<TKey, User<TKey>, TPermissionStore, TIdentityProvider>();
        }

        public static IServiceCollection AddPermissions<TKey, TUser, TPermissionStore, TIdentityProvider>(this IServiceCollection services)
            where TUser : class, IUser<TKey>
            where TPermissionStore : class, IPermissionStore
            where TIdentityProvider : class, IIdentityProvider<TKey>
        {
            services.AddSingleton<ResourceCollection>();
            services.AddSingleton<PermissionCache>();
            services.AddScoped<TUser>();
            services.AddScoped<IUser, TUser>();
            services.AddScoped<IUser<TKey>, TUser>();
            services.AddScoped<IPermissionStore, TPermissionStore>();
            services.AddScoped<IPermissionManager, PermissionManager>();
            services.AddSingleton<IIdentityProvider<TKey>, TIdentityProvider>();
            return services;
        }

        /// <summary>
        /// Register enum as permission resource
        /// </summary>
        public static IServiceCollection AddPermissionResource<TPermissionEnum>(this IServiceCollection services, int resourceUniqueId)
            where TPermissionEnum : IConvertible
        {
            var resource = new ResourceDefinition(typeof(TPermissionEnum), resourceUniqueId);
            services.AddSingleton(resource);
            return services;
        }


        /// <summary>
        /// Register enum as permission resource
        /// </summary>
        public static IServiceCollection AddPermissionResourceForInstances<TPermissionEnum, TResourceInstanceProvider>(this IServiceCollection services, int resourceUniqueId)
            where TPermissionEnum : IConvertible
            where TResourceInstanceProvider : class, IResourceInstanceProvider
        {
            var resource = new ResourceDefinition(typeof(TPermissionEnum), resourceUniqueId, typeof(TResourceInstanceProvider));
            services.AddSingleton(resource);
            services.AddScoped<IResourceInstanceProvider, TResourceInstanceProvider>();
            return services;
        }
    }
}
#endif