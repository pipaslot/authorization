using System;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class ResourceCollectionExtensionsTest
    {
        [Test]
        public void ToNumber_Perm1()
        {
            var resources = CreateResources();
            var value = resources.ToNumber(Perm1.DotIt);
            Assert.AreEqual(1, value.ResourceId);
            Assert.AreEqual((int)Perm1.DotIt, value.PermissionId);
        }

        [Test]
        public void ToNumber_Perm2()
        {
            var resources = CreateResources();
            var value = resources.ToNumber(Perm2.DotIt);
            Assert.AreEqual(2, value.ResourceId);
            Assert.AreEqual((int)Perm2.DotIt, value.PermissionId);
        }

        [Test]
        public void ToNumber_NotRegistered_ThrowException()
        {
            var resources = new ResourceCollection();
            Assert.Throws<Exception>(() =>
            {
                resources.ToNumber(Perm1.DotIt);
            });
        }

        [Test]
        public void ToEnum_Perm1()
        {
            var resources = CreateResources();
            var value = (Perm1)resources.ToEnum(1,1);
            Assert.AreEqual(Perm1.DotIt, value);
        }

        [Test]
        public void ToEnum_Perm2()
        {
            var resources = CreateResources();
            var value = (Perm2)resources.ToEnum(2,10);
            Assert.AreEqual(Perm2.DotIt, value);
        }

        [Test]
        public void ToEnum_Unknown_ThrowException()
        {
            var resources = CreateResources();
            Assert.Throws<Exception>(() =>
            {
                resources.ToEnum(2, 100);
            });
        }

        private ResourceCollection CreateResources()
        {
            var resources = new ResourceCollection();
            resources.Add<Perm1>(1);
            resources.Add<Perm2>(2);
            return resources;
        }

        private enum Perm1
        {
            DotIt = 1
        }
        private enum Perm2
        {
            DotIt = 10
        }
    }
}