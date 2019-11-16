
using System;
using System.Linq;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    /// <summary>
    /// Convert permission id into unique id and back
    /// </summary>
    public static class ResourceCollectionExtensions
    {
        public static int ToEnum(this ResourceCollection resources, int resourceId, int number)
        {
            var definition = resources.FirstOrDefault(r => r.ResourceId == resourceId);
            if (definition != null)
            {
                if (Enum.IsDefined(definition.PermissionEnumType, number))
                {
                    return number;
                }
                throw new Exception($"Enum {definition.PermissionEnumType} does not contains permission with value {number}");
            }
            throw new Exception($"Unknown permission with number {number}");
        }

        public static ResourcePermissionKey ToNumber(this ResourceCollection resources, IConvertible permission)
        {
            var type = permission.GetType();
            return resources.ToNumber(type, (int)permission);
        }

        public static ResourcePermissionKey ToNumber(this ResourceCollection resources, Type permissionEnum, int permissionValue)
        {
            var definition = resources.FirstOrDefault(r => r.PermissionEnumType == permissionEnum);
            if (definition != null)
            {
                return new ResourcePermissionKey
                {
                    ResourceId = definition.ResourceId,
                    PermissionId = (int)permissionValue
                };
            }

            throw new Exception($"Unknown permission '{permissionValue}' with type {permissionEnum}");
        }
    }
}
