using System.Collections.Generic;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    class UserTest_NoRoles : UserTestBase
    {
        [Test]
        public void IsAllowed_UserWithoutAnyRole_DoNotFailAndReturnFalse()
        {
            var user = SetupUser();
            var result = user.IsAllowed(Permission.DoIt);
            Assert.IsFalse( result);
        }

        [Test]
        public void IsAllowedForInstance_UserWithoutAnyRole_DoNotFailAndReturnFalse()
        {
            var user = SetupUser();
            var result = user.IsAllowed(Permission.DoIt,555);
            Assert.IsFalse( result);
        }
        
        [Test]
        public void CheckPermission_UserWithoutAnyRole_DoNotFailAndThrowException()
        {
            var user = SetupUser();
            Assert.Catch<AuthorizationException>(() =>
            {
                user.CheckPermission(Permission.DoIt);
            });
        }
        
        [Test]
        public void CheckPermissionForInstance_UserWithoutAnyRole_DoNotFailAndThrowException()
        {
            var user = SetupUser();
            Assert.Catch<AuthorizationException>(() =>
            {
                user.CheckPermission(Permission.DoIt,555);
            });
        }

        private User<int> SetupUser()
        {
            var (user, ip, pm) = CreateUserWithoutSystemRoles<int>();
            ip.Setup(i => i.GetRoles()).Returns<List<string>>(null);
            pm.Setup(p => p.GetSystemRoles()).Returns<List<string>>(null);
            return user;
        }
    }
}
