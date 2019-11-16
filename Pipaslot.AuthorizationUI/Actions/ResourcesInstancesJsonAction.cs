using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Models;
using Pipaslot.AuthorizationUI.ActionAbstraction;

namespace Pipaslot.AuthorizationUI.Actions
{
    class ResourcesInstancesJsonAction<TKey> : AJsonAction
    {
        private readonly TKey _role;
        private readonly int _rangeStart;

        public ResourcesInstancesJsonAction(TKey role, int rangeStart)
        {
            _role = role;
            _rangeStart = rangeStart;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var manager = (IPermissionManager<TKey>)services.GetService(typeof(IPermissionManager<TKey>));
            if (manager == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager<TKey>)} from Dependency Injection.");
            }

            var result = manager.GetResourceInstancePermissionsAsync(_role, _rangeStart).Result;

            return result.Select(ConvertToTemplateData).ToList();
        }

        private ResourceDto.Instance ConvertToTemplateData(ResourceInstancePermissions m)
        {
            return new ResourceDto.Instance
            {
                ResourceId = m.ResourceId,
                Identifier = m.InstanceId,
                Name = m.Name,
                Description = m.Description,
                Permissions = m.Permissions.Select(p=>new ResourceDto.Permission
                {
                    PermissionId = p.PermissionId,
                    Description = p.Description,
                    Name = p.Name,
                    IsAllowed = p.IsAllowed
                }).ToList()
            };
        }
    }
}
