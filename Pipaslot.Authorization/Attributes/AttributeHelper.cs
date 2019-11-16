using System;
using System.Globalization;
using System.Linq;

namespace Pipaslot.Authorization.Attributes
{
    public static class AttributeHelper
    {
        public static string GetPermissionName(IConvertible permissionEnum)
        {
            var field = permissionEnum.GetType().GetField(permissionEnum.ToString(CultureInfo.InvariantCulture));
            if (field.GetCustomAttributes(false).FirstOrDefault(a => a is StaticPermissionAttribute) is StaticPermissionAttribute nameAttr)
            {
                return nameAttr.Name;
            }
            return field.Name;
        }

        public static string GetResourceName(IConvertible permissionEnum)
        {
            var type = permissionEnum.GetType();
            if (type.GetCustomAttributes(false).FirstOrDefault(a => a is ResourceAttribute) is ResourceAttribute nameAttr)
            {
                return nameAttr.Name;
            }

            return type.Name;
        }

        public static ResourceMetadata GetResourceMetadata(Type permissionEnum)
        {
            var resourceAttr =
                permissionEnum.GetCustomAttributes(false).FirstOrDefault(a => a is ResourceAttribute) as
                    ResourceAttribute;
            var permissions = permissionEnum
                .GetFields()
                .Where(f=>f.IsLiteral)
                .Select(f =>
                {
                    var permissionAttr = f.GetCustomAttributes(false)
                        .FirstOrDefault(a => a is StaticPermissionAttribute) as StaticPermissionAttribute;

                    return new ResourceMetadata.Permission
                    {
                        Name = permissionAttr?.Name ?? f.Name,
                        Description = permissionAttr?.Description ?? "",
                        Identifier = (int)f.GetValue(null),
                        IsForInstance = permissionAttr is InstancePermissionAttribute
                    };
                })
                .ToList();
            var metadata = new ResourceMetadata
            {
                Name = resourceAttr?.Name ?? permissionEnum.Name,
                Description = resourceAttr?.Description ?? "",
                Permissions = permissions
            };
            return metadata;
        }
    }
}
