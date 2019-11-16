using System;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization;
using Pipaslot.AuthorizationUI.ActionAbstraction;

namespace Pipaslot.AuthorizationUI.Actions
{
    class PrivilegeJsonAction<TKey> : AJsonAction
    {
        private readonly TKey _role;
        private readonly int _resourceId;
        private readonly int _permissionId;
        private readonly string _instanceId;
        private readonly bool? _isAllowed;

        public PrivilegeJsonAction(TKey role, int resourceId, int permissionId, string instanceId, bool? isAllowed)
        {
            _role = role;
            _resourceId = resourceId;
            _permissionId = permissionId;
            _instanceId = instanceId;
            _isAllowed = isAllowed;
        }

        protected override object GetData(HttpContext context, IServiceProvider services)
        {
            var manager = (IPermissionManager<TKey>)services.GetService(typeof(IPermissionManager<TKey>));
            if (manager == null)
            {
                throw new ApplicationException($"Can not resolve service {typeof(IPermissionManager<TKey>)} from Dependency Injection.");
            }

            manager.SetPermission(_role, _resourceId, _permissionId, _instanceId, _isAllowed);

            return true;
        }
    }
}
