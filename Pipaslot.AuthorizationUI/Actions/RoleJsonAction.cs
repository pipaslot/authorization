using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Models;
using Pipaslot.AuthorizationUI.ActionAbstraction;

namespace Pipaslot.AuthorizationUI.Actions
{
    class RoleJsonAction<TKey> : AJsonAction
    {
        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var store = (IPermissionManager<TKey>)services.GetService(typeof(IPermissionManager<TKey>));
            if (store == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager<TKey>)} from Dependency Injection.");
            }
            var result = new List<object>();
            foreach (var role in store.GetAllRoles())
            {
                result.Add(new
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    ShowResources = role.Type != RoleType.Admin,
                    IsSystem = role.Type != RoleType.Custom
                });
            }
            return result;
        }
    }
}
