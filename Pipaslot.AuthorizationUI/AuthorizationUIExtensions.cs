using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Web;

namespace Pipaslot.AuthorizationUI
{
    public static class AuthorizationUIExtensions
    {
        public static IApplicationBuilder UseAuthorizationUI<TKey>(this IApplicationBuilder builder, Action<AuthorizationUIOptions> setup = null)
        {
            var options = new AuthorizationUIOptions();
            setup?.Invoke(options);
            return builder.UseMiddleware<AuthorizationUIMiddleware<TKey>>(options);
        }

        public static IServiceCollection AddAuthorizationUI<TKey, TPermissionStore>(this IServiceCollection services, int resourceUniqueId)
            where TPermissionStore : class, IPermissionStore<TKey>
        {
            services.AddPermissions<TKey, TPermissionStore>();
            services.AddPermissionResource<AuthorizationUIPermissions>(resourceUniqueId);
            return services;
        }
    }
}
