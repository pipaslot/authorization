using System;
using System.Collections.Generic;
using System.Linq;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    public class PermissionCache
    {
        private readonly Dictionary<string,RoleList> _roleCollection = new Dictionary<string,RoleList>();

        public bool? Load(Func<bool?> callback, ICollection<string> roleIds, ResourcePermissionKey permissionUid, string instanceId)
        {
            var sortedIds = roleIds.OrderBy(i => i).ToList();
            var key = string.Join("|#|", sortedIds);

            //Find or create roles
            if(!_roleCollection.TryGetValue(key, out var roleList))
            {
                _roleCollection[key] = roleList = new RoleList(sortedIds);
            }
            //Find or create permission
            var existingPermission = roleList.Permissions.FirstOrDefault(p => p.Uid == permissionUid && p.InstanceId == instanceId);
            if (existingPermission == null)
            {
                var isAllowed = callback();
                existingPermission = new Permission(permissionUid, instanceId, isAllowed);
                roleList.Permissions.Add(existingPermission);
            }
            return existingPermission.IsAllowed;
        }

        /// <summary>
        /// Remove cached record
        /// </summary>
        public void Clear(string roleId, ResourcePermissionKey permissionUid)
        {
            var roleLists = _roleCollection.Values.Where(r => r.Ids.Contains(roleId));
            foreach (var roleList in roleLists)
            {
                roleList.Permissions.RemoveAll(p => p.Uid == permissionUid);
            }
        }

        private class RoleList
        {
            public ICollection<string> Ids { get; }
            public List<Permission> Permissions { get; } = new List<Permission>();

            public RoleList(ICollection<string> ids)
            {
                Ids = ids;
            }
        }
        private class Permission
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
