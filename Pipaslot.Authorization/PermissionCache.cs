using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    public class PermissionCache<TKey>
    {
        private readonly List<RoleList<TKey>> _roleLists = new List<RoleList<TKey>>();

        public bool? Load(Func<bool?> callback, IEnumerable<TKey> roleIds, ResourcePermissionKey permissionUid, string instanceId)
        {
            var newRoles = new RoleList<TKey>(roleIds);
            //Find or create roles
            var existingRoles = _roleLists.FirstOrDefault(r => r.Key == newRoles.Key);
            if (existingRoles == null)
            {
                _roleLists.Add(newRoles);
                existingRoles = newRoles;
            }
            //Find or create permission
            var existingPermission = existingRoles.Permissions.FirstOrDefault(p => p.Uid == permissionUid && p.InstanceId == instanceId);
            if (existingPermission == null)
            {
                var isAllowed = callback();
                existingPermission = new Permission(permissionUid, instanceId, isAllowed);
                existingRoles.Permissions.Add(existingPermission);
            }
            return existingPermission.IsAllowed;
        }

        /// <summary>
        /// Remove cached record
        /// </summary>
        public void Clear(TKey roleId, ResourcePermissionKey permissionUid)
        {
            var roleLists = _roleLists.Where(r => r.Ids.Contains(roleId));
            foreach (var roleList in roleLists)
            {
                roleList.Permissions.RemoveAll(p => p.Uid == permissionUid);
            }
        }

        internal class RoleList<TKey2>
        {
            public string Key { get; }
            public List<TKey2> Ids { get; }
            public List<Permission> Permissions { get; } = new List<Permission>();

            public RoleList(IEnumerable<TKey2> ids)
            {
                Ids = ids.OrderBy(i => i).ToList();
                Key = string.Join("#", Ids);
            }
        }
        internal class Permission
        {
            public ResourcePermissionKey Uid { get; }
            public string InstanceId { get; }
            public bool? IsAllowed { get; }

            public Permission(ResourcePermissionKey uid, string instanceId, bool? isAllowed)
            {
                Uid = uid;
                InstanceId = instanceId;
                IsAllowed = isAllowed;
            }
        }
    }
}
