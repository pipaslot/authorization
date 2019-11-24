using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class UserTest_PermissionCheck : UserTestBase
    {
        [TestCase(true)]
        [TestCase(false)]
        public void IsAllowed_ReturnValueFromPermissionManager(bool isAllowed)
        {
            var userRoles = new List<string> { "customRole" };
            var permission = Permission.DoIt;

            var user = SetupUserForPermissionCheck<int>(isAllowed, userRoles, permission);

            var result = user.IsAllowed(permission);

            Assert.AreEqual(isAllowed, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsAllowedForInstance_ReturnValueFromPermissionManager(bool isAllowed)
        {
            var userRoles = new List<string> { "customRole" };
            var permission = Permission.DoIt;
            var instance = 1;

            var user = SetupUserForPermissionCheck<int>(isAllowed, userRoles, permission, instance);
            var result = user.IsAllowed(permission, instance);

            Assert.AreEqual(isAllowed, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CheckPermission_ReturnValueFromPermissionManager(bool isAllowed)
        {
            var userRoles = new List<string> { "customRole" };
            var permission = Permission.DoIt;

            var user = SetupUserForPermissionCheck<int>(isAllowed, userRoles, permission);

            if (isAllowed)
            {
                user.CheckPermission(permission);
            }
            else
            {
                Assert.Catch<AuthorizationException>(() =>
                {
                    user.CheckPermission(permission);
                });
            }
        }


        [TestCase(true)]
        [TestCase(false)]
        public void CheckPermissionForInstance_ReturnValueFromPermissionManager(bool isAllowed)
        {
            var userRoles = new List<string> { "customRole" };
            var permission = Permission.DoIt;
            var instance = 1;

            var user = SetupUserForPermissionCheck<int>(isAllowed, userRoles, permission, instance);

            if (isAllowed)
            {
                user.CheckPermission(permission, instance);
            }
            else
            {
                Assert.Catch<AuthorizationException>(() =>
                {
                    user.CheckPermission(permission, instance);
                });
            }
        }

        private User<TKey> SetupUserForPermissionCheck<TKey>(bool isAllowed, List<string> roles, Permission permission, TKey instance = default)
        {
            var userRoles = new List<string>(roles);
            roles.Add("guest");

            var (user, ip, pm) = CreateUserWithSystemRoles<TKey>();
            ip.Setup(i => i.GetRoles()).Returns(userRoles);
            pm.Setup(m => m.IsAllowed(roles, permission)).Returns(isAllowed);
            pm.Setup(m => m.IsAllowed<TKey>(roles, permission, instance)).Returns(isAllowed);

            return user;
        }
    }
}