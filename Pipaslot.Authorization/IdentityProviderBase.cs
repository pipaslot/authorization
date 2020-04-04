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

        public virtual TUserId GetUserId()
        {
            var user = GetClaimPrincipal();
            if (user == null)
            {
                return default;
            }
            var identityClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (identityClaim != null)
            {
                if (TryConvertId(identityClaim.Value, out var id))
                {
                    return id;
                }
            }
            var name = user.Identity?.Name;
            if (user.Identity is WindowsIdentity)
            {
                return WindowsUserNameToId(name);
            }

            if (TryConvertId(name, out var id2))
            {
                return id2;
            }
            throw new NotSupportedException($"Generic attribute TKey of type {typeof(TUserId)} is not supported. Only int, long and string can be used");
        }

        protected virtual bool TryConvertId(string stringId, out TUserId value)
        {
            value = default;
            if (stringId is TUserId targetValue)
            {
                value = targetValue;
                return true;
            }
            if (typeof(TUserId) == typeof(int))
            {
                value = ParseIntFromString(stringId);
                return true;
            }
            if (typeof(TUserId) == typeof(long))
            {
                value = ParseLongFromString(stringId);
                return true;
            }

            return false;
        }

        private TUserId ParseIntFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            if (int.TryParse(value, out var result))
            {
                return (TUserId)(object)result;
            }
            throw new ArgumentOutOfRangeException($"User ID: Expected integer value but got '{value}'");
        }

        private TUserId ParseLongFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            if (long.TryParse(value, out var result))
            {
                return (TUserId)(object)result;
            }
            throw new ArgumentOutOfRangeException($"User ID: Expected long value but got '{value}'");
        }

        public virtual bool IsAuthenticated => GetClaimPrincipal()?.Identity?.IsAuthenticated ?? false;

        public virtual List<string> GetRoles()
        {
            var user = GetClaimPrincipal();
            if (user == null)
            {
                return new List<string>();
            }
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
