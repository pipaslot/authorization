using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class UserTest_AssignRoles : UserTestBase
    {
        [TestCase(false, "custom")]
        [TestCase(false, "guest")]
        [TestCase(true, "custom")]
        [TestCase(true, "guest")]
        [TestCase(true, "user")]
        public void IsAllowed_AuthenticationStatus_ReturnCorrectRole(bool isAuthenticated, string expectedRole)
        {
            var context = SetupRoleTest<int>(isAuthenticated, "custom", Permission.DoIt);
            //Test
            context.User.IsAllowed(Permission.DoIt);

            //Verify
            context.PermissionManagerMock.Verify(p => p.IsAllowed(
                    It.Is<ICollection<string>>(c => c.Contains(expectedRole)), Permission.DoIt),
                Times.Once);
        }

        [TestCase(false, "custom")]
        [TestCase(false, "guest")]
        [TestCase(true, "custom")]
        [TestCase(true, "guest")]
        [TestCase(true, "user")]
        public void IsAllowedForInstance_AuthenticationStatus_ReturnCorrectRole(bool isAuthenticated, string expectedRole)
        {
            var instanceId = 1;
            var context = SetupRoleTest<int>(isAuthenticated, "custom", Permission.DoIt, instanceId);
            //Test
            context.User.IsAllowed<int>(Permission.DoIt, instanceId);

            //Verify
            context.PermissionManagerMock.Verify(p => p.IsAllowed<int>(
                    It.Is<ICollection<string>>(c => c.Contains(expectedRole)), Permission.DoIt, instanceId),
                Times.Once);
        }

        [TestCase(false, "custom")]
        [TestCase(false, "guest")]
        [TestCase(true, "custom")]
        [TestCase(true, "guest")]
        [TestCase(true, "user")]
        public void CheckPermission_AuthenticationStatus_ReturnCorrectRole(bool isAuthenticated, string expectedRole)
        {
            var context = SetupRoleTest<int>(isAuthenticated, "custom", Permission.DoIt);
            //Test
            context.User.CheckPermission(Permission.DoIt);

            //Verify
            context.PermissionManagerMock.Verify(p => p.IsAllowed(
                    It.Is<ICollection<string>>(c => c.Contains(expectedRole)), Permission.DoIt),
                Times.Once);
        }

        [TestCase(false, "custom")]
        [TestCase(false, "guest")]
        [TestCase(true, "custom")]
        [TestCase(true, "guest")]
        [TestCase(true, "user")]
        public void CheckPermissionForInstance_AuthenticationStatus_ReturnCorrectRole(bool isAuthenticated, string expectedRole)
        {
            var instanceId = 1;
            var context = SetupRoleTest<int>(isAuthenticated, "custom", Permission.DoIt, instanceId);
            //Test
            context.User.CheckPermission<int>(Permission.DoIt, instanceId);

            //Verify
            context.PermissionManagerMock.Verify(p => p.IsAllowed(
                    It.Is<ICollection<string>>(c => c.Contains(expectedRole)), Permission.DoIt, instanceId),
                Times.Once);
        }

        private (User<TKey> User, Mock<IPermissionManager> PermissionManagerMock) SetupRoleTest<TKey>(
            bool isAuthenticated, string customRole, Permission permission, TKey identifier = default)
        {
            var identityProviderMock = new Mock<IIdentityProvider<TKey>>(MockBehavior.Strict);
            var permissionManagerMock = new Mock<IPermissionManager>(MockBehavior.Loose);
            var user = new User<TKey>(identityProviderMock.Object, permissionManagerMock.Object);

            //Setup 
            identityProviderMock.Setup(i => i.IsAuthenticated).Returns(isAuthenticated);
            identityProviderMock.Setup(i => i.GetRoles()).Returns(new List<string> {customRole});
            permissionManagerMock.Setup(p => p.GetSystemRoles()).Returns(SystemRoles);

            permissionManagerMock.Setup(p => p.IsAllowed(It.IsAny<ICollection<string>>(), permission)).Returns(true);
            permissionManagerMock.Setup(p => p.IsAllowed<TKey>(It.IsAny<ICollection<string>>(), permission, identifier))
                .Returns(true);
            return (user, permissionManagerMock);
        }
    }
}