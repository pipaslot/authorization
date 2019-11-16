using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipaslot.AuthorizationUI
{
    /// <summary>
    /// Describes structure used on frontend for permission management
    /// </summary>
    internal class ResourceDto
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }

        public IReadOnlyList<Permission> Permissions { get; internal set; } = new List<Permission>();

        public bool HasInstancePermissions { get; set; }
        public int ResourceId { get; set; }

        public Instance[] Instances = new Instance[0];
        public bool ShowPermissions { get; set; }
        public bool ShowInstances { get; set; }

        public class Instance
        {
            public string Identifier { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool ShowPermissions { get; set; }
            public IReadOnlyList<Permission> Permissions { get; internal set; } = new List<Permission>();
            public int ResourceId { get; set; }
        }

        public class Permission
        {
            public string Name { get; internal set; }
            public string Description { get; internal set; }
            public int PermissionId { get; internal set; }
            public bool? IsAllowed { get; internal set; }
        }
    }
}
