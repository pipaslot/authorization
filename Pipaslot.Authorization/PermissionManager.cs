using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipaslot.Authorization.Attributes;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    public class PermissionManager : IPermissionManager
    {
        private readonly ResourceCollection _resources;
        private readonly IPermissionStore _permissionStore;
        private readonly PermissionCache _cache;
        private readonly IEnumerable<IResourceInstanceProvider> _resourceInstanceProviders;

        public PermissionManager(ResourceCollection resources, IPermissionStore permissionStore, PermissionCache cache, IEnumerable<IResourceInstanceProvider> resourceInstanceProviders)
        {
            _resources = resources;
            _permissionStore = permissionStore;
            _cache = cache;
            _resourceInstanceProviders = resourceInstanceProviders;
        }

        public bool IsAllowed(ICollection<string> roles, IConvertible permissionEnum)
        {
            var permissionUid = _resources.ToNumber(permissionEnum);
            var isAllowed = _cache.Load(
                () => _permissionStore.IsAllowed(roles, permissionUid, null),
                roles,
                permissionUid,
                null);
            return isAllowed ?? false;
        }

        public bool IsAllowed<TInstanceKey>(ICollection<string> roles, IConvertible permissionEnum, TInstanceKey instanceKey)
        {
            var permissionUid = _resources.ToNumber(permissionEnum);
            var instance = instanceKey?.ToString();
            var isAllowed = _cache.Load(
                () => _permissionStore.IsAllowed(roles, permissionUid, instance),
                roles,
                permissionUid,
                instance);
            if (isAllowed == null)
            {
                isAllowed = _cache.Load(
                    () => _permissionStore.IsAllowed(roles, permissionUid, null),
                    roles,
                    permissionUid,
                    null);
            }

            return isAllowed ?? false;
        }

        public async Task SetPermission(string role, int resourceId, int permissionId, bool? isEnabled)
        {
            _cache.Clear(role, new ResourcePermissionKey
            {
                ResourceId = resourceId,
                PermissionId = permissionId
            });
            await _permissionStore.SetPermissionAsync(role, resourceId, permissionId, null, isEnabled);
        }


        public async Task SetPermission(string role, int resourceId, int permissionId, string instanceId, bool? isEnabled)
        {
            _cache.Clear(role, new ResourcePermissionKey
            {
                ResourceId = resourceId,
                PermissionId = permissionId
            });
            await _permissionStore.SetPermissionAsync(role, resourceId, permissionId, instanceId, isEnabled);
        }

        public ICollection<IRoleDetail> GetAllRoles()
        {
            return _permissionStore.GetAllRoles();
        }

        public ICollection<IRole> GetSystemRoles()
        {
            return _permissionStore.GetSystemRoles();
        }
        
        public IReadOnlyList<ResourceDetail> GetAllResources()
        {
            return _resources
                .Select(ConvertResource)
                .OrderBy(r => r.Name)
                .ToList();
        }

        public async Task<IReadOnlyList<ResourcePermissions>> GetResourcePermissionsAsync(string roleId)
        {
            var permissions = await _permissionStore.GetRoleStaticPermissionsAsync(roleId);
            return _resources
                .Select(r => ConvertResource(r, permissions))
                .OrderBy(r => r.Name)
                .ToList();
        }

        private ResourceDetail ConvertResource(ResourceDefinition resource)
        {
            var metadata = AttributeHelper.GetResourceMetadata(resource.PermissionEnumType);
            var perms = new List<ResourceDetail.Permission>();
            foreach (var permission in metadata.Permissions)
            {
                var uid = _resources.ToNumber(resource.PermissionEnumType, permission.Identifier);
                perms.Add(new ResourceDetail.Permission
                {
                    Description = permission.Description,
                    Name = permission.Name,
                    PermissionId = uid.PermissionId
                });
            }
            return new ResourceDetail
            {
                Name = metadata.Name,
                Description = metadata.Description,
                ResourceId = resource.ResourceId,
                Permissions = perms,
                HasInstancePermissions = metadata.Permissions.Any(p => p.IsForInstance)
            };
        }

        private ResourcePermissions ConvertResource(ResourceDefinition resource, Dictionary<ResourcePermissionKey, bool> permissions)
        {
            var metadata = AttributeHelper.GetResourceMetadata(resource.PermissionEnumType);
            var perms = new List<ResourcePermissions.Permission>();
            foreach (var permission in metadata.Permissions)
            {
                var uid = _resources.ToNumber(resource.PermissionEnumType, permission.Identifier);
                var perm = new ResourcePermissions.Permission
                {
                    Description = permission.Description,
                    Name = permission.Name,
                    PermissionId = uid.PermissionId
                };
                if (permissions.TryGetValue(uid, out var isAllowed))
                {
                    perm.IsAllowed = isAllowed;
                }
                perms.Add(perm);
            }
            return new ResourcePermissions
            {
                Name = metadata.Name,
                Description = metadata.Description,
                ResourceId = resource.ResourceId,
                Permissions = perms,
                HasInstancePermissions = metadata.Permissions.Any(p => p.IsForInstance)
            };
        }

        public async Task<IReadOnlyList<ResourceInstancePermissions>> GetResourceInstancePermissionsAsync(string roleId, int resourceId)
        {
            var definition = _resources.FirstOrDefault(d => d.ResourceId == resourceId);
            if (definition == null)
            {
                throw new ApplicationException($"Can not find resource with ID {resourceId}");
            }

            var dataProviderType = definition.ResourceInstanceProviderTypeOrNull;
            if (dataProviderType == null)
            {
                throw new ApplicationException($"Data provider not specified for resource {definition.PermissionEnumType}");
            }

            var dataProvider = _resourceInstanceProviders.FirstOrDefault(r => r.GetType() == dataProviderType);
            if (dataProvider == null)
            {
                throw new ApplicationException($"Can not resolve service {dataProviderType} from Dependency Injection.");
            }

            var instances = await dataProvider.GetAllInstancesAsync();

            var permissions = await _permissionStore.GetRoleInstancePermissionsAsync(roleId, resourceId);
            var metadata = AttributeHelper.GetResourceMetadata(definition.PermissionEnumType);
            var permissionsMetadata = metadata.Permissions.Where(p => p.IsForInstance).ToList();
            return instances
                .Select(r => ConvertInstance(r, definition.PermissionEnumType, permissionsMetadata, permissions))
                .OrderBy(r => r.Name)
                .ToList();
        }

        private ResourceInstancePermissions ConvertInstance(ResourceInstance instance, Type permissionEnumType, List<ResourceMetadata.Permission> metadata, Dictionary<InstancePermissionKey, bool> permissions)
        {
            var perms = new List<ResourceInstancePermissions.Permission>();
            var resourceId = 0;
            foreach (var permissionMeta in metadata)
            {
                if (!permissionMeta.IsForInstance) continue;
                var uid = _resources.ToNumber(permissionEnumType, permissionMeta.Identifier);
                resourceId = uid.ResourceId;
                var perm = new ResourceInstancePermissions.Permission
                {
                    Description = permissionMeta.Description,
                    Name = permissionMeta.Name,
                    PermissionId = uid.PermissionId
                };
                var key = new InstancePermissionKey
                {
                    ResourceId = uid.ResourceId,
                    PermissionId = uid.PermissionId,
                    InstanceId = instance.Id
                };

                if (permissions.TryGetValue(key, out var isAllowed))
                {
                    perm.IsAllowed = isAllowed;
                }

                perms.Add(perm);
            }
            return new ResourceInstancePermissions
            {
                ResourceId = resourceId,
                InstanceId = instance.Id,
                Name = instance.Name,
                Description = instance.Description,
                Permissions = perms
            };
        }
    }
}
