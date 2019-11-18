using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Authorization.Web
{
    public static class ServiceCollectionExtension
    {
        /// <typeparam name="TKey">User primary key type</typeparam>
        /// <typeparam name="TPermissionStore">Database layer for permissions</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPermissions<TKey, TPermissionStore>(this IServiceCollection services)
            where TPermissionStore : class, IPermissionStore
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddPermissions<TKey, TPermissionStore, IdentityProvider<TKey>>();

            return services;
        }
    }
}
