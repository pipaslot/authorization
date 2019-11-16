using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Authorization.Models
{
    public class ResourcePermissions
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public int ResourceId { get; set; }

        public IReadOnlyList<Permission> Permissions { get; internal set; } = new List<Permission>();
        public bool HasInstancePermissions { get; internal set; }

        public class Permission
        {
            public string Name { get; internal set; }
            public string Description { get; internal set; }
            public int PermissionId { get; internal set; }
            public bool? IsAllowed { get; internal set; }
        }
    }
}
