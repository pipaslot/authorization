using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    public interface IPermissionManager
    {

        /// <summary>
        /// Automatically resolve resource and check if user has permission.
        /// </summary>
        bool IsAllowed(ICollection<string> roles, IConvertible permissionEnum);

        /// <summary>
        /// Automatically resolve resource and check if user has permission for its instance.
        /// </summary>
        bool IsAllowed<TInstanceKey>(ICollection<string> roles, IConvertible permissionEnum, TInstanceKey instanceKey);

        /// <summary>
        /// Read all static permissions for role with resource and permission details
        /// </summary>
        Task<IReadOnlyList<ResourcePermissions>> GetResourcePermissionsAsync(string roleId);

        /// <summary>
        /// Read all instance permissions for role with resource and permission details
        /// </summary>
        Task<IReadOnlyList<ResourceInstancePermissions>> GetResourceInstancePermissionsAsync(string roleId, int resourceId);

        /// <summary>
        /// Set role permission
        /// </summary>
        Task SetPermission(string role, int resourceId, int permissionId, bool? isEnabled);

        /// <summary>
        /// Set role permission
        /// </summary>
        Task SetPermission(string role, int resourceId, int permissionId, string instanceId, bool? isEnabled);

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
