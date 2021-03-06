﻿using System;
using NUnit.Framework;

namespace Pipaslot.Authorization.Tests
{
    public class ResourceCollectionTest
    {
        [Test]
        public void Add_AddTwo_Pass()
        {
            var collection = new ResourceCollection();
            collection.Add<Perm1>(0);
            collection.Add<Perm2>(2);
            Assert.AreEqual(2, collection.Count);
        }

        [Test]
        public void Add_TheSameResourceId_ThrowException()
        {
            var collection = new ResourceCollection();
            collection.Add<Perm1>(1);
            Assert.Throws<Exception>(() => { collection.Add<Perm2>(1); });
        }

        [Test]
        public void Add_TheSameEnumType_ThrowException()
        {
            var collection = new ResourceCollection();
            collection.Add<Perm1>(0); 
            Assert.Throws<Exception>(() => { collection.Add<Perm1>(1); });
        }

        [Test]
        public void Add_WithDuplicateKeys_ThrowException()
        {
            var collection = new ResourceCollection();
            Assert.Throws<Exception>(() => { collection.Add<PermWitDuplicates>(1); });
        }

        [Test]
        public void Constructor_AddTwo_Pass()
        {
            var collection = new ResourceCollection(new[]
            {
                new ResourceDefinition(typeof(Perm1),0),
                new ResourceDefinition(typeof(Perm2),1)
            });
            Assert.AreEqual(2, collection.Count);
        }

        [Test]
        public void Constructor_TheSameResourceId_ThrowException()
        {
            Assert.Throws<Exception>(() =>
            {
                new ResourceCollection(new[]
                {
                    new ResourceDefinition(typeof(Perm1),1),
                    new ResourceDefinition(typeof(Perm2),1)
                });
            });
        }

        [Test]
        public void Constructor_TheSameEnumType_ThrowException()
        {
            Assert.Throws<Exception>(() =>
            {
                new ResourceCollection(new[]
                {
                    new ResourceDefinition(typeof(Perm1),0),
                    new ResourceDefinition(typeof(Perm1),1)
                });
            });
        }

        [Test]
        public void Constructor_WithDuplicateKeys_ThrowException()
        {
            Assert.Throws<Exception>(() =>
            {
                new ResourceCollection(new[]
                {
                    new ResourceDefinition(typeof(PermWitDuplicates),1)
                });
            });
        }

        private enum Perm1
        {
            DoIt=10
        }
        private enum Perm2
        {

        }

        private enum PermWitDuplicates
        {
            DoIt = 10,
            DoItTheSameWay = 10
        }
    }
}
