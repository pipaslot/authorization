using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Http;

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
            if (_contextAccessor.HttpContext.User.Identity is WindowsIdentity)
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

        protected virtual TUserId WindowsUserNameToId(string name)
        {
            throw new NotImplementedException($"Username conversion for Windows identity is not implemented. Override method {nameof(WindowsUserNameToId)} to add windows identity conversion support");
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

        public bool IsAuthenticated => _contextAccessor.HttpContext.User.Identity.IsAuthenticated;

        public List<string> GetRoles()
        {
            var user = _contextAccessor.HttpContext.User;
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
