using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    /// <inheritDoc />
    public class User<TKey> : IUser<TKey>
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly IPermissionManager<TKey> _permissionManager;

        public User(IClaimsPrincipalProvider claimsPrincipalProvider, IPermissionManager<TKey> permissionManager)
        {
            _claimsPrincipalProvider = claimsPrincipalProvider;
            _permissionManager = permissionManager;
        }

        public bool IsAuthenticated => _claimsPrincipalProvider.GetClaimsPrincipal().Identity.IsAuthenticated;

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
            var identity = GetIdentity();
            return identity.Roles.ContainsAdmin() || _permissionManager.IsAllowed(identity.Roles.GetIds<TKey>(), permissionEnum);
        }

        public bool IsAllowed<TInstanceKey>(IConvertible permissionEnum, TInstanceKey instanceKey)
        {
            var identity = GetIdentity();
            return identity.Roles.ContainsAdmin() || _permissionManager.IsAllowed(identity.Roles.GetIds<TKey>(), permissionEnum, instanceKey);
        }
        
        public TKey Id => GetIdentity().Id;

        private (TKey Id, IEnumerable<IRole> Roles) GetIdentity()
        {
            var user = _claimsPrincipalProvider.GetClaimsPrincipal();
            var roleClaims = user.FindAll(ClaimTypes.Role);
            //Load assigned not system user roles from claims
            var roles = roleClaims.Select(_claimsPrincipalProvider.ClaimToRole).ToList();
            var systemRoles = GetSystemRoles();
            //Auto assign guest role
            roles.Add(systemRoles.First(r => r.Type == RoleType.Guest));
            var name = user.Identity?.Name;
            //If username is not empty, add User role
            if (!string.IsNullOrWhiteSpace(name))
            {
                roles.Add(systemRoles.First(r => r.Type == RoleType.User));
            }
            if (typeof(TKey) == typeof(int))
            {
                foreach (var role in roles)
                {
                    role.Id = ParseIntFromString(role.Id?.ToString(), "User ID");
                }
                return (Id: ParseIntFromString(name, "Role ID"), Roles: roles);
            }
            if (typeof(TKey) == typeof(long))
            {
                foreach (var role in roles)
                {
                    role.Id = ParseLongFromString(role.Id?.ToString(), "User ID");
                }
                return (Id: ParseLongFromString(name, "Role ID"), Roles: roles);
            }
            if (typeof(TKey) == typeof(string))
            {
                return (Id: (TKey)(object)name, Roles: roles);
            }
            throw new NotSupportedException($"Generic attribute TKey of type {typeof(TKey)} is not supported. Only int, long and string can be used");
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

        private TKey ParseIntFromString(string value, string field)
        {
            try
            {
                return string.IsNullOrWhiteSpace(value) ? default : (TKey)(object)int.Parse(value);
            }
            catch (FormatException)
            {
                throw new ArgumentOutOfRangeException($"{field}: Expected integer value but got '{value}'");
            }
        }


        private TKey ParseLongFromString(string value, string field)
        {
            try
            {
                return string.IsNullOrWhiteSpace(value) ? default : (TKey)(object)long.Parse(value);
            }
            catch (FormatException)
            {
                throw new ArgumentOutOfRangeException($"{field}: Expected long value but got '{value}'");
            }
        }
    }
}
