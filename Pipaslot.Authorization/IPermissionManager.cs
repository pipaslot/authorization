using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    public interface IPermissionManager<TKey>
    {

        /// <summary>
        /// Automatically resolve resource and check if user has permission.
        /// </summary>
        bool IsAllowed(ICollection<TKey> roles, IConvertible permissionEnum);

        /// <summary>
        /// Automatically resolve resource and check if user has permission for its instance.
        /// </summary>
        bool IsAllowed<TInstanceKey>(ICollection<TKey> roles, IConvertible permissionEnum, TInstanceKey instanceKey);

        /// <summary>
        /// Read all static permissions for role with resource and permission details
        /// </summary>
        Task<IReadOnlyList<ResourcePermissions>> GetResourcePermissionsAsync(TKey roleId);

        /// <summary>
        /// Read all instance permissions for role with resource and permission details
        /// </summary>
        Task<IReadOnlyList<ResourceInstancePermissions>> GetResourceInstancePermissionsAsync(TKey roleId, int resourceId);

        /// <summary>
        /// Set role permission
        /// </summary>
        Task SetPermission(TKey role, int resourceId, int permissionId, bool? isEnabled);

        /// <summary>
        /// Set role permission
        /// </summary>
        Task SetPermission(TKey role, int resourceId, int permissionId, string instanceId, bool? isEnabled);

        /// <summary>
        /// Returns all roles
        /// </summary>
        ICollection<IRole> GetAllRoles();

        /// <summary>
        /// Returns all roles with type to be Guest, User or Admin
        /// </summary>
        ICollection<IRole> GetSystemRoles();
    }
}
