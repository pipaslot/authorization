using System;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class UserTest_ArgumentCheck : UserTestBase
    {
        
        [Test]
        public void IsAllowed_PermissionIsNull_ExceptionIsThrown()
        {
            var (user, _, _) = CreateUserWithSystemRoles<int>();
            Assert.Catch<ArgumentException>(() =>
            {
                user.IsAllowed(null);
            });
        }

        [Test]
        public void IsAllowedForInstance_PermissionIsNull_ExceptionIsThrown()
        {
            var (user, _, _) = CreateUserWithSystemRoles<int>();
            Assert.Catch<ArgumentException>(() =>
            {
                user.IsAllowed<int>(null, 0);
            });
        }

        [Test]
        public void CheckPermission_PermissionIsNull_ExceptionIsThrown()
        {
            var (user, _, _) = CreateUserWithSystemRoles<int>();
            Assert.Catch<ArgumentException>(() =>
            {
                user.CheckPermission(null);
            });
        }

        [Test]
        public void CheckPermissionForInstance_PermissionIsNull_ExceptionIsThrown()
        {
            var (user, _, _) = CreateUserWithSystemRoles<int>();
            Assert.Catch<ArgumentException>(() =>
            {
                user.CheckPermission<int>(null, 0);
            });
        }
    }
}