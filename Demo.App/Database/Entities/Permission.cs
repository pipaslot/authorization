using Demo.App.Database.Entities;

namespace Demo.App.Database.Entities
{
    /// <summary>
    /// Allowed permissions
    /// </summary>
    public class Permission
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
        public int PermissionId { get; set; }
        public int ResourceId { get; set; }
        public string InstanceId { get; set; }
        public bool IsAllowed { get; set; }
    }
}
