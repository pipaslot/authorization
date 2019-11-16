using System;
using System.Text;
using Pipaslot.Authorization.Attributes;

namespace Pipaslot.Authorization
{
    public class AuthorizationException : UnauthorizedAccessException
    {
        /// <summary>
        /// Required permission
        /// </summary>
        public IConvertible Permission { get; }

        /// <summary>
        /// User friendly name for required permission
        /// </summary>
        public string PermissionName => AttributeHelper.GetPermissionName(Permission);
        
        /// <summary>
        /// User friendly resource name
        /// </summary>
        public string ResourceName => AttributeHelper.GetResourceName(Permission);

        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException(IConvertible permissionEnum)
        {
            Permission = permissionEnum;
        }
        
        public override string Message => $"You do not have permission: '{PermissionName}' for resource: '{ResourceName}'";
    }
}