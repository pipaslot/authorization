﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    /// <summary>
    /// Storage used for storing privileges for resource permissions
    /// Must be designed as singleton
    /// </summary>
    public interface IPermissionStore
    {
        /// <summary>
        /// Check permission assigned assigned to role
        /// </summary>
        bool? IsAllowed(string roleId, ResourcePermissionKey permissionUid, string instanceId);

        /// <summary>
        /// Check instance permission assigned assigned to role
        /// </summary>
        bool? IsAllowed(ICollection<string> roleIds, ResourcePermissionKey permissionUid, string instanceId);

        /// <summary>
        /// Grant Permission for role and resource instance
        /// </summary>
        Task SetPermissionAsync(string role, int resourceId, int permissionId, string instanceId, bool? isAllowed, CancellationToken token = default);

        /// <summary>
        /// Read all stored static permissions for role
        /// </summary>
        /// <returns>Unique permission id as key and flag if is allowed as value</returns>
        Task<Dictionary<ResourcePermissionKey, bool>> GetRoleStaticPermissionsAsync(string roleId, CancellationToken token = default);

        /// <summary>
        /// Read all stored instance permission for role
        /// </summary>
        /// <returns>Resource id as first key, Unique permission id as second key and flag if is allowed as value</returns>
        Task<Dictionary<InstancePermissionKey, bool>> GetRoleInstancePermissionsAsync(string roleId, int resourceId, CancellationToken token = default);

        /// <summary>
        /// Returns all roles
        /// </summary>
        ICollection<IRoleDetail> GetAllRoles();

        /// <summary>
        /// Returns all roles with type to be Guest, User or Admin
        /// </summary>
        ICollection<IRole> GetSystemRoles();
    }
}
