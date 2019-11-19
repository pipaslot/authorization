using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Web;
// ReSharper disable InconsistentNaming

namespace Pipaslot.AuthorizationUI
{
    public static class AuthorizationUIExtensions
    {
        /// <summary>
        /// Register middleware exposing permission management for existing roles
        /// </summary>
        public static IApplicationBuilder UseAuthorizationUI(this IApplicationBuilder builder, Action<AuthorizationUIOptions> setup = null)
        {
            var options = new AuthorizationUIOptions();
            setup?.Invoke(options);
            return builder.UseMiddleware<AuthorizationUIMiddleware>(options);
        }

        /// <summary>
        /// Configure serviced for Authorization middleware
        /// </summary>
        /// <typeparam name="TUserId">User primary key type</typeparam>
        /// <typeparam name="TPermissionStore"></typeparam>
        /// <param name="services"></param>
        /// <param name="resourceUniqueId">Unique resource ID for permissions belonging to authorization UI</param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationUI<TUserId, TPermissionStore>(this IServiceCollection services, int resourceUniqueId)
            where TPermissionStore : class, IPermissionStore
        {
            services.AddPermissions<TUserId, TPermissionStore>();
            services.AddPermissionResource<AuthorizationUIPermissions>(resourceUniqueId);
            return services;
        }
    }
}
