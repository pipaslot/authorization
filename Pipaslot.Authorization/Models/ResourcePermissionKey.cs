using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Authorization.Models
{
    public struct ResourcePermissionKey
    {
        public int ResourceId;

        public int PermissionId;

        public static bool operator ==(ResourcePermissionKey c1, ResourcePermissionKey c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(ResourcePermissionKey c1, ResourcePermissionKey c2)
        {
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return ResourceId.GetHashCode() ^ PermissionId.GetHashCode();
        }
    }
}
