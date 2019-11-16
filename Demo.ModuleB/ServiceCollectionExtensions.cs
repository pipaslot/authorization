using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pipaslot.Authorization;

namespace Demo.ModuleB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModuleB(this IServiceCollection services, int resourceUniqueId)
        {
            services.AddPermissionResource<ModuleBPermission>(resourceUniqueId);
            return services;
        }
    }
}
