using System;
using System.Security.Claims;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class IdentityProviderBaseTest_GetId
    {
        [Test]
        public void GetUserId_PreferNameIdentifier()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "123"),
                new Claim(ClaimTypes.NameIdentifier, "456")
            };
            var sut = new IdentityProvider<string>(claims);
            var id = sut.GetUserId();
            Assert.AreEqual("456", id);
        }

        [TestCase(ClaimTypes.Name, "123", (int)123)]
        [TestCase(ClaimTypes.NameIdentifier, "123", (int)123)]
        public void GetUserId_IntType_ReturnValue(string claimType, string claimValue, int expectedValue)
        {
            GetUserId_GenericTest(claimType, claimValue, expectedValue);
        }
        
        [TestCase(ClaimTypes.Name, "123", (long)123)]
        [TestCase(ClaimTypes.NameIdentifier, "123", (long)123)]
        public void GetUserId_LongType_ReturnValue(string claimType, string claimValue, long expectedValue)
        {
            GetUserId_GenericTest(claimType, claimValue, expectedValue);
        }
        
        [TestCase(ClaimTypes.Name, "123", "123")]
        [TestCase(ClaimTypes.NameIdentifier, "123", "123")]
        public void GetUserId_StringType_ReturnValue(string claimType, string claimValue, string expectedValue)
        {
            GetUserId_GenericTest(claimType, claimValue, expectedValue);
        }
        
        private void GetUserId_GenericTest<TKey>(string claimType, string claimValue, TKey expectedValue)
        {
            var sut = new IdentityProvider<TKey>(claimType, claimValue);
            var id = sut.GetUserId();
            Assert.AreEqual(expectedValue, id);
        }

        private class IdentityProvider<TUserId> : IdentityProviderBase<TUserId>
        {
            private readonly ClaimsPrincipal _claimsPrincipal;
            
            public IdentityProvider(Claim[] claims)
            {
                var identity = new ClaimsIdentity(claims);
                _claimsPrincipal = new ClaimsPrincipal(identity);
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
