using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class UserTest_Properties : UserTestBase
    {
        
        [TestCase(true)]
        [TestCase(false)]
        public void IsAuthenticated_ReturnValueFromIdentityProvider(bool isAuthenticated)
        {
            var (user, ip, _) = CreateUserWithSystemRoles<int>();
            ip.Setup(i => i.IsAuthenticated).Returns(isAuthenticated);
            Assert.AreEqual(isAuthenticated, user.IsAuthenticated);
        }

        [Test]
        public void Id_PermissionIsNull_ExceptionIsThrown()
        {
            var userId = 555;
            var (user, ip, _) = CreateUserWithSystemRoles<int>();
            
            ip.Setup(i => i.GetUserId()).Returns(userId);
            Assert.AreEqual(userId, user.Id);
        }
    }
}