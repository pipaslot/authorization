using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Web;

namespace Pipaslot.AuthorizationUI
{
    public static class AuthorizationUIExtensions
    {
        public static IApplicationBuilder UseAuthorizationUI(this IApplicationBuilder builder, Action<AuthorizationUIOptions> setup = null)
        {
            var options = new AuthorizationUIOptions();
            setup?.Invoke(options);
            return builder.UseMiddleware<AuthorizationUIMiddleware>(options);
        }

        public static IServiceCollection AddAuthorizationUI<TKey, TPermissionStore>(this IServiceCollection services, int resourceUniqueId)
            where TPermissionStore : class, IPermissionStore
        {
            services.AddPermissions<TKey, TPermissionStore>();
            services.AddPermissionResource<AuthorizationUIPermissions>(resourceUniqueId);
            return services;
        }
    }
}
