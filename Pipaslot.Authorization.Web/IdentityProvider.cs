using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization.Web
{
    public class IdentityProvider<TUserId> : IIdentityProvider<TUserId>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public TUserId GetUserId()
        {
            var name = _contextAccessor.HttpContext.User.Identity?.Name;
            if (name is TUserId targetValue)
            {
                return targetValue;
            }
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("\\\\"))
            {
                return WindowsUserNameToId(name);
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

        protected virtual TUserId WindowsUserNameToId(string name)
        {
            if (name is TUserId stringName)
            {
                return stringName;
            }
            return default;
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

        /// <summary>
        /// Delimiter used for role claim to joint role parameters into one string
        /// </summary>
        private const string RoleClaimFieldDelimiter = "|#|";

        public bool IsAuthenticated => _contextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public List<IRole> GetRoles()
        {
            var user = _contextAccessor.HttpContext.User;
            var roles = user
                .FindAll(ClaimTypes.Role)
                .Select(ClaimToRole)
                .Where(r => r != null)
                .ToList();

            if (user.Identity is WindowsIdentity win)
            {
                var groups = win.Groups?.ToArray() ?? new IdentityReference[0];
                var windowsRoles = groups
                    .Select(id => id.Translate(typeof(NTAccount)).Value)
                    .Select(WindowsRoleToRole)
                    .Where(r => r != null)
                    .ToArray();
                roles.AddRange(windowsRoles);
            }
            return roles;
        }

        protected virtual IRole WindowsRoleToRole(string windowsRole)
        {
            return null;
        }

        public Claim RoleToClaim(IRole role)
        {
            var roleClaim = string.Join(RoleClaimFieldDelimiter, role.Id?.ToString(), ((int)role.Type).ToString());
            return new Claim(ClaimTypes.Role, roleClaim);
        }

        public IRole ClaimToRole(Claim claim)
        {
            var roleFields = claim.Value.Split(new[] { RoleClaimFieldDelimiter }, StringSplitOptions.None);
            var role = new Role { Id = roleFields[0] };
            if (roleFields.Length >= 2 && Enum.TryParse<RoleType>(roleFields[1], out var type))
            {
                role.Type = type;
            }
            return role;
        }

        private class Role : IRole
        {
            public string Id { get; set; }

            public RoleType Type { get; set; } = RoleType.Custom;
        }
    }
}
