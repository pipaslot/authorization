namespace Pipaslot.Authorization.Models
{
    public interface IRoleDetail : IRole
    {
        /// <summary>
        /// Role name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Role description
        /// </summary>
        string Description { get; }
    }

    public interface IRole
    {
        string Id { get; set; }

        /// <summary>
        /// Specify role type 
        /// </summary>
        RoleType Type { get; }
    }
}
