using System;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    public class PermissionCache
    {
        private readonly List<RoleList> _roleLists = new List<RoleList>();

        public bool? Load(Func<bool?> callback, IEnumerable<string> roleIds, ResourcePermissionKey permissionUid, string instanceId)
        {
            var newRoles = new RoleList(roleIds);
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
        public void Clear(string roleId, ResourcePermissionKey permissionUid)
        {
            var roleLists = _roleLists.Where(r => r.Ids.Contains(roleId));
            foreach (var roleList in roleLists)
            {
                roleList.Permissions.RemoveAll(p => p.Uid == permissionUid);
            }
        }

        internal class RoleList
        {
            public string Key { get; }
            public List<string> Ids { get; }
            public List<Permission> Permissions { get; } = new List<Permission>();

            public RoleList(IEnumerable<string> ids)
            {
                Ids = ids.OrderBy(i => i).ToList();
                Key = string.Join("|#|", Ids);
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
