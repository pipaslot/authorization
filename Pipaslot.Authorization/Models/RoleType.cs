using System;
using System.Collections.Generic;
using System.Text;

namespace Pipaslot.Authorization.Models
{
    public enum RoleType
    {
        /// <summary>
        /// Role assigned to all visitors
        /// </summary>
        Guest = 1,

        /// <summary>
        /// Role assigned only for authenticated users
        /// </summary>
        User = 2,

        /// <summary>
        /// Role for administrator with all permissions
        /// </summary>
        Admin = 3,

        /// <summary>
        /// Application specific role with full maintanance
        /// </summary>
        Custom = 4
    }
}
