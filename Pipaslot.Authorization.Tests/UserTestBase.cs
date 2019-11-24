using Moq;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization.Tests
{
    public class UserTestBase
    {
        protected readonly IRole[] SystemRoles = {
            new Role
            {
                Id = "guest",
                Type = RoleType.Guest
            },
            new Role
            {
                Id = "user",
                Type = RoleType.User
            },
            new Role
            {
                Id = "admin",
                Type = RoleType.Admin
            }
        };
        
        protected (User<TKey> User, Mock<IIdentityProvider<TKey>> IdentityProviderMock, Mock<IPermissionManager> PermissionManager) 
            CreateUserWithoutSystemRoles<TKey>()
        {
            var identityProviderMock = new Mock<IIdentityProvider<TKey>>();
            var permissionManagerMock = new Mock<IPermissionManager>();
            var user = new User<TKey>(identityProviderMock.Object, permissionManagerMock.Object);

            return (user, identityProviderMock, permissionManagerMock);
        }
        protected (User<TKey> User, Mock<IIdentityProvider<TKey>> IdentityProviderMock, Mock<IPermissionManager> PermissionManager) 
            CreateUserWithSystemRoles<TKey>()
        {
            var (user, identity, permMan) = CreateUserWithoutSystemRoles<TKey>();
            permMan.Setup(p => p.GetSystemRoles()).Returns(SystemRoles);

            return (user,identity,permMan);
        }

        protected enum Permission
        {
            DoIt
        }

        protected class Role : IRole
        {
            public string Id { get; set; }
            public RoleType Type { get; set; }
        }
    }
}
