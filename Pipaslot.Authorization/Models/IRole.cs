using System;
using System.Collections.Generic;
using System.Text;

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
        object Id { get; set; }

        /// <summary>
        /// Specificate role type 
        /// </summary>
        RoleType Type { get; }
    }
}
