using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization.Tests
{
    public class PermissionCacheTest
    {
        [Test]
        public void Load_CallOnce_LoadDataFromStore()
        {
            var sut = new PermissionCache();
            var count = 0;
            var roles = new List<string> { "Role1", "Role2" };
            var permission = new ResourcePermissionKey
            {
                PermissionId = 1,
                ResourceId = 2
            };
            var instance = "0";
            sut.Load(() => { count++; return true; }, roles, permission, instance);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Load_CallTwiceWithTheSameDataInTheSameRoleOrder_OnlyOnceCallsStore()
        {
            var sut = new PermissionCache();
            var count = 0;
            var roles = new List<string> { "Role1", "Role2" };
            var permission = new ResourcePermissionKey
            {
                PermissionId = 1,
                ResourceId = 2
            };
            var instance = "0";
            sut.Load(() => { count++; return true; }, roles, permission, instance);
            sut.Load(() => { count++; return true; }, roles, permission, instance);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Load_CallTwiceWithTheSameDataInDifferentRoleOrder_OnlyOnceCallsStore()
        {
            var sut = new PermissionCache();
            var count = 0;
            var roles1 = new List<string> { "Role1", "Role2" };
            var roles2 = new List<string> { "Role2", "Role1" };
            var permission = new ResourcePermissionKey
            {
                PermissionId = 1,
                ResourceId = 2
            };
            var instance = "0";
            sut.Load(() => { count++; return true; }, roles1, permission, instance);
            sut.Load(() => { count++; return true; }, roles2, permission, instance);
            Assert.AreEqual(1, count);
        }

        [TestCase("Role1", 2)]
        [TestCase("Role2", 2)]
        [TestCase("NonExistingRole", 1)]
        public void Clear_CallTwiceWithTheSameDataInDifferentRoleOrder_OnlyOnceCallsStore(string clearedRole, int expectedCalls)
        {
            var sut = new PermissionCache();
            var count = 0;
            var roles = new List<string> { "Role1", "Role2" };
            var permission = new ResourcePermissionKey
            {
                PermissionId = 1,
                ResourceId = 2
            };
            var instance = "0";
            sut.Load(() => { count++; return true; }, roles, permission, instance);
            sut.Clear(clearedRole, permission);
            sut.Load(() => { count++; return true; }, roles, permission, instance);
            Assert.AreEqual(expectedCalls, count);
        }
    }
}
