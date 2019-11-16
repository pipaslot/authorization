using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Models;
using Pipaslot.AuthorizationUI.ActionAbstraction;

namespace Pipaslot.AuthorizationUI.Actions
{
    class ResourcesJsonAction<TKey> : AJsonAction
    {
        private readonly TKey _role;

        public ResourcesJsonAction(TKey role)
        {
            _role = role;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var manager = (IPermissionManager<TKey>)services.GetService(typeof(IPermissionManager<TKey>));
            if (manager == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager<TKey>)} from Dependency Injection.");
            }

            var result = manager.GetResourcePermissionsAsync(_role).Result;

            return result.Select(ConvertToTemplateData).ToList();
        }

        private ResourceDto ConvertToTemplateData(ResourcePermissions m)
        {
            return new ResourceDto
            {
                Name = m.Name,
                Description = m.Description,
                ResourceId = m.ResourceId,
                Permissions = m.Permissions
                    .Select(p => new ResourceDto.Permission
                    {
                        PermissionId = p.PermissionId,
                        Name = p.Name,
                        Description = p.Description,
                        IsAllowed = p.IsAllowed
                    }).ToList(),
                HasInstancePermissions = m.HasInstancePermissions
            };
        }
    }
}
