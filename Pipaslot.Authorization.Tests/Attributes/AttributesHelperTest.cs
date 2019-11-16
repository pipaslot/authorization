using System.Linq;
using NUnit.Framework;
using Pipaslot.Authorization.Attributes;

namespace Pipaslot.Authorization.Tests.Attributes
{
    public class AttributesHelperTest
    {
        [Test]
        public void GetPermissionName_NamedStatic_ReturnAttributeName()
        {
            var name = AttributeHelper.GetPermissionName(NamedPermissions.DoIt);
            Assert.AreEqual("PermissionName", name);
        }
        [Test]
        public void GetPermissionName_NamedInstance_ReturnAttributeName()
        {
            var name = AttributeHelper.GetPermissionName(NamedPermissions.DoItOnInstance);
            Assert.AreEqual("InstancePermissionName", name);
        }
        [Test]
        public void GetPermissionName_Anonymous_ReturnPropertyName()
        {
            var name = AttributeHelper.GetPermissionName(AnonymousPermissions.DoIt);
            Assert.AreEqual(nameof(AnonymousPermissions.DoIt), name);
        }

        [Test]
        public void GetResourceName_Named_ReturnAttributeName()
        {
            var name = AttributeHelper.GetResourceName(NamedPermissions.DoIt);
            Assert.AreEqual("ResourceName", name);
        }

        [Test]
        public void GetResourceName_Anonymous_ReturnClassName()
        {
            var name = AttributeHelper.GetResourceName(AnonymousPermissions.DoIt);
            Assert.AreEqual(nameof(AnonymousPermissions), name);
        }

        [Test]
        public void GetResourceMetadata_Named_ReturnAttributeName()
        {
            var metadata = AttributeHelper.GetResourceMetadata(typeof(NamedPermissions));
            Assert.AreEqual("ResourceName", metadata.Name);
            Assert.AreEqual("ResourceDescription", metadata.Description);
            Assert.AreEqual(2, metadata.Permissions.Count);

            var permission1 = metadata.Permissions.First(p => p.Identifier == (int)NamedPermissions.DoIt);
            Assert.AreEqual("PermissionName", permission1.Name);
            Assert.AreEqual("PermissionDescription", permission1.Description);
            Assert.IsFalse(permission1.IsForInstance);

            var permission2 = metadata.Permissions.First(p => p.Identifier == (int)NamedPermissions.DoItOnInstance);
            Assert.AreEqual("InstancePermissionName", permission2.Name);
            Assert.AreEqual("InstancePermissionDescription", permission2.Description);
            Assert.IsTrue(permission2.IsForInstance);
        }

        [Test]
        public void GetResourceMetadata_Anonymous_ReturnClassName()
        {
            var metadata = AttributeHelper.GetResourceMetadata(typeof(AnonymousPermissions));
            Assert.AreEqual(nameof(AnonymousPermissions), metadata.Name);
            Assert.AreEqual("", metadata.Description);
            Assert.AreEqual(1, metadata.Permissions.Count);

            var permission = metadata.Permissions.First();
            Assert.AreEqual(nameof(AnonymousPermissions.DoIt), permission.Name);
            Assert.AreEqual("", permission.Description);
            Assert.AreEqual((int)AnonymousPermissions.DoIt, permission.Identifier);
            Assert.IsFalse(permission.IsForInstance);
        }

        [Resource("ResourceName", "ResourceDescription")]
        private enum NamedPermissions
        {
            [StaticPermission("PermissionName", "PermissionDescription")]
            DoIt = 1,
            [InstancePermission("InstancePermissionName", "InstancePermissionDescription")]
            DoItOnInstance = 2
        }

        private enum AnonymousPermissions
        {

            DoIt = 1
        }
    }
}
