using System.Collections.Generic;
using Pipaslot.Authorization.Models;

namespace Demo.App.Database.Entities
{
    public class Role
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleType Type { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}