using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Authorization.Models
{
    public static class IRoleExtensions
    {
        public static bool ContainsAdmin(this IEnumerable<IRole> roles)
        {
            return roles.Any(r => r.Type == RoleType.Admin);
        }

        public static List<TKey> GetIds<TKey>(this IEnumerable<IRole> roles)
        {
            return roles.Select(r => (TKey)r.Id).ToList();
        }
    }
}
