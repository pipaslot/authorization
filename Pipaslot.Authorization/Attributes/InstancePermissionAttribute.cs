using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Authorization.Attributes
{
    /// <summary>
    /// Permission applicable only for instances
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InstancePermissionAttribute : StaticPermissionAttribute
    {
        public InstancePermissionAttribute(string name = null, string description = null) : base(name, description)
        {
        }
    }
}
