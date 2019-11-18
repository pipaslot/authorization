using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.App.Database.Entities;
using Demo.App.Models;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Models;

namespace Demo.App.Database
{
    public class PermissionStore : IPermissionStore<long>
    {
        private readonly DatabaseFactory _databaseFactory;

        public PermissionStore(DatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public bool? IsAllowed(long roleId, ResourcePermissionKey permissionUid, string instanceId)
        {
            return IsAllowed(new[] { roleId }, permissionUid, instanceId);
        }

        public bool? IsAllowed(ICollection<long> roleIds, ResourcePermissionKey permissionUid, string instanceId)
        {
            using (var db = _databaseFactory.Create())
            {
                var privilege = db.Permission.FirstOrDefault(p => roleIds.Contains(p.RoleId)
                                                                             && p.ResourceId == permissionUid.ResourceId
                                                                             && p.PermissionId == permissionUid.PermissionId
                    && p.InstanceId == instanceId);
                return privilege?.IsAllowed;
            }
        }

        public async Task SetPermissionAsync(long role, int resourceId, int permissionId, string instanceId, bool? isAllowed, CancellationToken token = default)
        {
            using (var db = _databaseFactory.Create())
            {
                var existing = db.Permission.FirstOrDefault(p => p.RoleId == role
                                                                 && p.ResourceId == resourceId
                                                                 && p.PermissionId == permissionId
                                                                 && p.InstanceId == instanceId);
                if (isAllowed == null)
                {
                    //unset
                    if (existing != null)
                    {
                        db.Permission.Remove(existing);
                    }

                    await db.SaveChangesAsync(token);
                    return;
                }
                if (existing == null)
                {
                    existing = new Permission()
                    {
                        RoleId = role,
                        ResourceId = resourceId,
                        PermissionId = permissionId,
                        InstanceId = instanceId,
                        IsAllowed = isAllowed ?? false
                    };
                    db.Permission.Add(existing);
                }
                else
                {
                    existing.IsAllowed = isAllowed ?? false;
                }
                await db.SaveChangesAsync(token);
            }
        }

        public async Task<Dictionary<ResourcePermissionKey, bool>> GetRoleStaticPermissionsAsync(long roleId, CancellationToken token = default)
        {
            using (var db = _databaseFactory.Create())
            {
                return await db.Permission
                    .Where(p => p.RoleId == roleId && p.InstanceId == null)
                    .GroupBy(p => new ResourcePermissionKey
                    {
                        ResourceId = p.ResourceId,
                        PermissionId = p.PermissionId
                    }, p => p.IsAllowed)
                    .ToDictionaryAsync(p => p.Key, p => p.First(), token);
            }
        }

        public async Task<Dictionary<InstancePermissionKey, bool>> GetRoleInstancePermissionsAsync(long roleId, int resourceId, CancellationToken token = default)
        {
            using (var db = _databaseFactory.Create())
            {
                var permissions = await db.Permission
                    .Where(p => p.RoleId == roleId
                                && resourceId == p.ResourceId
                                && p.InstanceId != null)
                    .GroupBy(p => new InstancePermissionKey
                    {
                        ResourceId = p.ResourceId,
                        PermissionId = p.PermissionId,
                        InstanceId = p.InstanceId
                    }, p => p.IsAllowed)
                    .ToDictionaryAsync(p => p.Key, p => p.First(), token);

                return permissions;
            }
        }

        public ICollection<IRoleDetail> GetAllRoles()
        {
            using (var db = _databaseFactory.Create())
            {
                var roles = db.Role.Select(r => (IRoleDetail)new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Type = r.Type
                }).ToList();
                return roles;
            }
        }

        public ICollection<IRole> GetSystemRoles()
        {
            using (var db = _databaseFactory.Create())
            {
                var roles = db.Role
                    .Where(r => r.Type == RoleType.Guest || r.Type == RoleType.User || r.Type == RoleType.Admin)
                    .Select(r => (IRole)new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Type = r.Type
                    })
                    .ToList();
                if (roles.Count != 3 || roles.Count(r => r.Type == RoleType.Guest) != 1 || roles.Count(r => r.Type == RoleType.User) != 1 || roles.Count(r => r.Type == RoleType.Admin) != 1)
                {
                    throw new ApplicationException($"System roles were not correctly configured. Were expected 3 roles but found {roles.Count}. Into application can be defined only one {nameof(RoleType.Guest)}, {nameof(RoleType.User)} and  {nameof(RoleType.Admin)} role type");
                }
                return roles.ToList();
            }
        }
    }
}
