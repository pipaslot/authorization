using System;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    /// <inheritDoc />
    public class User<TKey> : IUser<TKey>
    {
        private readonly IIdentityProvider<TKey> _identityProvider;
        private readonly IPermissionManager _permissionManager;

        public User(IIdentityProvider<TKey> identityProvider, IPermissionManager permissionManager)
        {
            _identityProvider = identityProvider;
            _permissionManager = permissionManager;
        }

        public bool IsAuthenticated => _identityProvider.IsAuthenticated;

        public void CheckPermission(IConvertible permissionEnum)
        {
            if (!IsAllowed(permissionEnum))
            {
                throw new AuthorizationException(permissionEnum);
            }
        }

        public void CheckPermission<TInstanceKey>(IConvertible permissionEnum, TInstanceKey instanceKey)
        {
            if (!IsAllowed(permissionEnum, instanceKey))
            {
                throw new AuthorizationException(permissionEnum);
            }
        }

        public bool IsAllowed(IConvertible permissionEnum)
        {
            var roles = GetRoles();
            return ContainsAdmin(roles) || _permissionManager.IsAllowed(roles.Select(r => r.Id).ToArray(), permissionEnum);
        }

        public bool IsAllowed<TInstanceKey>(IConvertible permissionEnum, TInstanceKey instanceKey)
        {
            var roles = GetRoles();
            return ContainsAdmin(roles) || _permissionManager.IsAllowed(roles.Select(r => r.Id).ToArray(), permissionEnum, instanceKey);
        }
        private bool ContainsAdmin(IEnumerable<IRole> roles)
        {
            var adminRole = GetSystemRoles()
                .FirstOrDefault(r => r.Type == RoleType.Admin);
            if (adminRole != null)
            {
                return roles.Any(r => r.Id == adminRole.Id);
            }
            return false;
        }

        public TKey Id => _identityProvider.GetUserId();

        private ICollection<IRole> GetRoles()
        {
            //Load assigned not system user roles from claims
            var roles = _identityProvider.GetRoles();
            var systemRoles = GetSystemRoles();
            //Auto assign guest role
            roles.Add(systemRoles.First(r => r.Type == RoleType.Guest));
            //If username is not empty, add User role
            if (_identityProvider.IsAuthenticated)
            {
                roles.Add(systemRoles.First(r => r.Type == RoleType.User));
            }
            return roles;
        }

        /// <summary>
        /// System role cache
        /// </summary>
        private ICollection<IRole> _systemRolesCache;

        /// <summary>
        /// Load system roles from database and cache them
        /// </summary>
        private ICollection<IRole> GetSystemRoles()
        {
            if (_systemRolesCache == null)
            {
                _systemRolesCache = _permissionManager.GetSystemRoles();
            }

            return _systemRolesCache;
        }
    }
}
