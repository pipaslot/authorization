using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Authorization;

namespace Demo.ModuleA
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModuleA<TResourceInstanceProvider>(this IServiceCollection services, int resourceUniqueId)
            where TResourceInstanceProvider : class, IResourceInstanceProvider
        {
            services.AddSingleton<ModuleAService>();
            services.AddPermissionResourceForInstances<ModuleAPermission, TResourceInstanceProvider>(resourceUniqueId);
            return services;
        }
    }
}
