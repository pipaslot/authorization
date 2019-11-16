using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Authorization.Attributes
{
    public class ResourceMetadata
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public IReadOnlyList<Permission> Permissions { get; internal set; } = new List<Permission>();

        public class Permission
        {
            public string Name { get; internal set; }
            public string Description { get; internal set; }
            public int Identifier { get; internal set; }
            public bool IsForInstance { get; set; }
        }
    }
}
