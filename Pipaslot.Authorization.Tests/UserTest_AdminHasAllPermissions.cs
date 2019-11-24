using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    class UserTest_AdminHasAllPermissions : UserTestBase
    {
        [TestCase("UnknownRole", false)]
        [TestCase("admin", true)]
        public void IsAllowed_IfSystemAdminRole_DoNotQueryStoreAndReturnTrue(string role, bool isAllowed)
        {
            var user = SetupAdminTest(role);
            var result = user.IsAllowed(Permission.DoIt);
            Assert.AreEqual(isAllowed, result);
        }

        [TestCase("UnknownRole", false)]
        [TestCase("admin", true)]
        public void IsAllowedForInstance_IfSystemAdminRole_DoNotQueryStoreAndReturnTrue(string role, bool isAllowed)
        {
            var user = SetupAdminTest(role);
            var result = user.IsAllowed(Permission.DoIt, 555);
            Assert.AreEqual(isAllowed, result);
        }

        [TestCase("UnknownRole", false)]
        [TestCase("admin", true)]
        public void CheckPermission_IfSystemAdminRole_DoNotQueryStoreAndReturnTrue(string role, bool isAllowed)
        {
            var user = SetupAdminTest(role);
            if (isAllowed)
            {
                user.CheckPermission(Permission.DoIt);
            }
            else
            {
                Assert.Catch<AuthorizationException>(() =>
                {
                    user.CheckPermission(Permission.DoIt);
                });
            }
        }

        [TestCase("UnknownRole", false)]
        [TestCase("admin", true)]
        public void CheckPermissionForInstance_IfSystemAdminRole_DoNotQueryStoreAndReturnTrue(string role, bool isAllowed)
        {
            var user = SetupAdminTest(role);
            if (isAllowed)
            {
                user.CheckPermission(Permission.DoIt, 555);
            }
            else
            {
                Assert.Catch<AuthorizationException>(() =>
                {
                    user.CheckPermission(Permission.DoIt, 555);
                });
            }
        }

        private User<int> SetupAdminTest(string role)
        {
            var (user, ip, _) = CreateUserWithSystemRoles<int>();
            ip.Setup(i => i.GetRoles()).Returns(new List<string> { role });
            return user;
        }
    }
}
