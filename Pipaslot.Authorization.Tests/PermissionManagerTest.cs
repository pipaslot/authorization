using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pipaslot.Authorization.Attributes;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization.Tests
{
    public class PermissionManagerTest
    {
        [Test]
        public async Task GetRersourcePermissions()
        {
            var roleId = "1";
            var resourceCollection = new ResourceCollection();
            resourceCollection.Add<Perm1>(1);
            resourceCollection.Add<Perm2>(2);
            var permissionStoreMock = new Mock<IPermissionStore>();
            permissionStoreMock.Setup(s => s.GetRoleStaticPermissionsAsync(roleId, CancellationToken.None))
                .Returns(Task.FromResult(new Dictionary<ResourcePermissionKey, bool>()));

            var service = new PermissionManager(resourceCollection, permissionStoreMock.Object, new PermissionCache(), new IResourceInstanceProvider[0]);
            var resources = await service.GetResourcePermissionsAsync(roleId);

            Assert.AreEqual(2, resources.Count);
            Assert.AreNotEqual(0, resources.First().Permissions.First().PermissionId);
        }

        [Test]
        public async Task GetResourceInstancePermissions()
        {
            var roleId = "1";
            var resourceId = 2;
            var permissions = new Dictionary<InstancePermissionKey, bool>()
            {
                {
                    new InstancePermissionKey
                    {
                        ResourceId = 2,
                        PermissionId = 1,
                        InstanceId = "1"
                    }, true
                }
            };
            var resourceCollection = new ResourceCollection();
            resourceCollection.Add<Perm1, FakeResourceInstanceProvider>(resourceId);
            var permissionStoreMock = new Mock<IPermissionStore>();
            permissionStoreMock.Setup(s => s.GetRoleInstancePermissionsAsync(roleId,resourceId, CancellationToken.None))
                .Returns(Task.FromResult(permissions));
            var providers = new[] {new FakeResourceInstanceProvider()};
            var service = new PermissionManager(resourceCollection, permissionStoreMock.Object, new PermissionCache(), providers);
            var resources = await service.GetResourceInstancePermissionsAsync(roleId, resourceId);

            Assert.AreEqual(3, resources.Count);
            var firstResource = resources.First(r => r.InstanceId == "I1");
            Assert.AreNotEqual(0, firstResource.Permissions.First().PermissionId);
        }

        private enum Perm1
        {
            [InstancePermission("Do it")]
            DoIt = 1
        }

        private enum Perm2
        {
        }

        private class FakeResourceInstanceProvider : IResourceInstanceProvider
        {
            public Task<ICollection<ResourceInstance>> GetAllInstancesAsync()
            {
                var collection = new[]
                {
                    new ResourceInstance
                    {
                        Id = "I1",
                        Name = "first"
                    },
                    new ResourceInstance
                    {
                        Id = "I2",
                        Name = "second"
                    },
                    new ResourceInstance
                    {
                        Id = "I3",
                        Name = "third"
                    }
                };
                return Task.FromResult<ICollection<ResourceInstance>>(collection);
            }
        }
    }
}
