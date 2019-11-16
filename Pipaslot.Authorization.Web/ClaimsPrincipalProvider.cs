using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization.Web
{
    public class ClaimsPrincipalProvider : IClaimsPrincipalProvider
    {

        /// <summary>
        /// Delimiter used for role claim to joint role parameters into one string
        /// </summary>
        private const string RoleClaimFieldDelimiter = "|#|";

        private readonly IHttpContextAccessor _contextAccessor;

        public ClaimsPrincipalProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ClaimsPrincipal GetClaimsPrincipal()
        {
            return _contextAccessor.HttpContext.User;
        }

        public Claim RoleToClaim(IRole role)
        {
            var roleClaim = string.Join(RoleClaimFieldDelimiter, role.Name, role.Id?.ToString(), ((int)role.Type).ToString());
            return new Claim(ClaimTypes.Role, roleClaim);
        }

        public IRole ClaimToRole(Claim claim)
        {
            var roleFields = claim.Value.Split(new[] { RoleClaimFieldDelimiter }, StringSplitOptions.None);
            var role = new Role { Name = roleFields[0] };
            if (roleFields.Length >= 2)
            {
                role.Id = roleFields[1];
            }
            if (roleFields.Length >= 3 && Enum.TryParse<RoleType>(roleFields[2], out var type))
            {
                role.Type = type;
            }
            return role;
        }

        private class Role : IRole
        {
            public object Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public RoleType Type { get; set; } = RoleType.Custom;
        }
    }
}
