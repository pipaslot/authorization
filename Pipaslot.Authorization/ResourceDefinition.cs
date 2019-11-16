using System;

namespace Pipaslot.Authorization
{
    public class ResourceDefinition
    {
        public Type PermissionEnumType { get; }
        public int ResourceId { get; }
        public Type ResourceInstanceProviderTypeOrNull { get; }

        public ResourceDefinition(Type permissionEnumType, int resourceId, Type resourceInstanceProviderType = null)
        {
            PermissionEnumType = permissionEnumType;
            ResourceId = resourceId;
            ResourceInstanceProviderTypeOrNull = resourceInstanceProviderType;
        }
    }
}