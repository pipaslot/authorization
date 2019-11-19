using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Pipaslot.Authorization
{
    public abstract class IdentityProviderBase<TUserId> : IIdentityProvider<TUserId>
    {
        protected abstract ClaimsPrincipal GetClaimPrincipal();

        protected abstract TUserId WindowsUserNameToId(string name);

        public TUserId GetUserId()
        {
            var user = GetClaimPrincipal();
            var name = user.Identity?.Name;
            if (user.Identity is WindowsIdentity)
            {
                return WindowsUserNameToId(name);
            }
            if (name is TUserId targetValue)
            {
                return targetValue;
            }
            if (typeof(TUserId) == typeof(int))
            {
                return ParseIntFromString(name, "Role ID");
            }
            if (typeof(TUserId) == typeof(long))
            {
                return ParseLongFromString(name, "Role ID");
            }
            throw new NotSupportedException($"Generic attribute TKey of type {typeof(TUserId)} is not supported. Only int, long and string can be used");
        }

        protected TUserId ParseIntFromString(string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            if (int.TryParse(value, out var result))
            {
                return (TUserId)(object)result;
            }
            throw new ArgumentOutOfRangeException($"{field}: Expected integer value but got '{value}'");
        }

        protected TUserId ParseLongFromString(string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            if (long.TryParse(value, out var result))
            {
                return (TUserId)(object)result;
            }
            throw new ArgumentOutOfRangeException($"{field}: Expected long value but got '{value}'");
        }

        public bool IsAuthenticated => GetClaimPrincipal().Identity.IsAuthenticated;

        public List<string> GetRoles()
        {
            var user = GetClaimPrincipal();
            var roles = user
                .FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToList();

            if (user.Identity is WindowsIdentity win)
            {
                var groups = win.Groups?.ToArray() ?? new IdentityReference[0];
                var windowsRoles = groups
                    .Select(id => id.Translate(typeof(NTAccount)).Value)
                    .Where(group => group.Contains("\\"))
                    .ToArray();
                roles.AddRange(windowsRoles);
            }
            return roles;
        }
    }
}
