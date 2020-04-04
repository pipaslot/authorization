using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class IdentityProviderBaseTest_UserIdentityIsNull
    {
        [Test]
        public void IsAuthenticated_UserIdentityIsNull_ReturnsFalse()
        {
            var sut = new NullIdentityProvider<int>();
            Assert.IsFalse(sut.IsAuthenticated);
        }

        [Test]
        public void GetRoles_UserIdentityIsNull_ReturnsEmptyCollection()
        {
            var sut = new NullIdentityProvider<int>();
            var roles = sut.GetRoles();
            Assert.AreEqual(0, roles.Count);
        }

        [Test]
        public void GetUserId_UserIdentityIsNull_ReturnsDefaultValue()
        {
            var sut = new NullIdentityProvider<int>();
            Assert.AreEqual(0, sut.GetUserId());
        }

        private class NullIdentityProvider<TUserId> : IdentityProviderBase<TUserId>
        {
            protected override ClaimsPrincipal GetClaimPrincipal()
            {
                return null;
            }

            protected override TUserId WindowsUserNameToId(string name)
            {
                throw new NotImplementedException();
            }
        }
    }
}
