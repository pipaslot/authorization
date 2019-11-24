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
            if (permissionEnum == null)
            {
                throw new ArgumentException("Permission can not be null");
            }
            var roles = GetRoles();
            return ContainsAdmin(roles) || _permissionManager.IsAllowed(roles, permissionEnum);
        }

        public bool IsAllowed<TInstanceKey>(IConvertible permissionEnum, TInstanceKey instanceKey)
        {
            if (permissionEnum == null)
            {
                throw new ArgumentException("Permission can not be null");
            }
            var roles = GetRoles();
            return ContainsAdmin(roles) || _permissionManager.IsAllowed(roles, permissionEnum, instanceKey);
        }
        private bool ContainsAdmin(IEnumerable<string> roles)
        {
            if (TryGetSystemRole(RoleType.Admin, out var roleAdmin))
            {
                return roles.Any(r => r == roleAdmin);
            }
            return false;
        }

        public TKey Id => _identityProvider.GetUserId();

        private ICollection<string> GetRoles()
        {
            //Load assigned not system user roles from claims
            var roles = _identityProvider.GetRoles() ?? new List<string>();
            if (TryGetSystemRole(RoleType.Guest, out var roleGuest))
            {
                roles.Add(roleGuest);
            }
            //If username is not empty, add User role
            if (_identityProvider.IsAuthenticated)
            {
                if (TryGetSystemRole(RoleType.User, out var roleUser))
                {
                    roles.Add(roleUser);
                }
            }
            return roles
                .Distinct()
                .ToList();
        }
        
        private bool TryGetSystemRole(RoleType type, out string roleId)
        {
            var role = GetSystemRoles().FirstOrDefault(r => r.Type == type);
            roleId = null;
            if (role != null)
            {
                roleId = role.Id;
                return true;
            }
            return false;
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

            return _systemRolesCache ?? new IRole[0];
        }
    }
}
