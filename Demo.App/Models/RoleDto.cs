using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipaslot.Authorization.Models;

namespace Demo.App.Models
{
    public class RoleDto : IRoleDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleType Type { get; set; }
    }
}
