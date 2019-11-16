using System.Collections.Generic;

namespace Demo.App.Database.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }
}