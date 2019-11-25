using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class IdentityProviderBaseTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void IsAuthenticated_UserFlagFromIdentity(bool isAuthenticated)
        {
            var identity = new ClaimsIdentity(isAuthenticated ? "Bearer" : null);
            var principal = new ClaimsPrincipal(identity);
            var sut = new IdentityProvider<int>(principal);
            Assert.AreEqual(isAuthenticated, sut.IsAuthenticated);
        }

        [TestCase("role1")]
        [TestCase("role2")]
        public void GetRoles_UserRoleClaims_ReturnsAllRoles(string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "role1"),
                new Claim(ClaimTypes.Role, "role2")
            };
            var sut = new IdentityProvider<int>(claims);
            var roles = sut.GetRoles();
            Assert.IsTrue(roles.Contains(role), $"Claim for role {role} not extracted");
        }

        private class IdentityProvider<TUserId> : IdentityProviderBase<TUserId>
        {
            private readonly ClaimsPrincipal _claimsPrincipal;

            public IdentityProvider(ClaimsPrincipal principal)
            {
                _claimsPrincipal = principal;
            }

            public IdentityProvider(string claimType, string claimValue)
            {
                var claims = new[]
                {
                    new Claim(claimType, claimValue)
                };
                var identity = new ClaimsIdentity(claims);
                _claimsPrincipal = new ClaimsPrincipal(identity);
            }

            public IdentityProvider(Claim[] claims)
            {
                var identity = new ClaimsIdentity(claims);
                _claimsPrincipal = new ClaimsPrincipal(identity);
            }

            protected override ClaimsPrincipal GetClaimPrincipal()
            {
                return _claimsPrincipal;
            }

            protected override TUserId WindowsUserNameToId(string name)
            {
                throw new NotImplementedException();
            }
        }
    }
}
