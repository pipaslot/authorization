using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissions<TKey, TPermissionStore, TClaimsPrincipalProvider>(this IServiceCollection services)
            where TClaimsPrincipalProvider : class, IClaimsPrincipalProvider
            where TPermissionStore : class, IPermissionStore<TKey>
        {
            return services.AddPermissions<TKey, User<TKey>, TPermissionStore, TClaimsPrincipalProvider>();
        }

        public static IServiceCollection AddPermissions<TKey, TUser, TPermissionStore, TClaimsPrincipalProvider>(this IServiceCollection services)
            where TUser : class, IUser<TKey>
            where TPermissionStore : class, IPermissionStore<TKey>
            where TClaimsPrincipalProvider : class, IClaimsPrincipalProvider
        {
            services.AddSingleton<ResourceCollection>();
            services.AddSingleton<PermissionCache<TKey>>();
            services.AddScoped<TUser>();
            services.AddScoped<IUser, TUser>();
            services.AddScoped<IUser<TKey>, TUser>();
            services.AddScoped<IPermissionStore<TKey>, TPermissionStore>();
            services.AddScoped<IPermissionManager<TKey>, PermissionManager<TKey>>();
            services.AddSingleton<IClaimsPrincipalProvider, TClaimsPrincipalProvider>();
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
