using System;
using System.Collections.Generic;
using System.Text;
using Pipaslot.Authorization;

namespace Demo.ModuleA
{
    public static class ResourceCollectionExtensions
    {
        public static ResourceCollection AddModuleA(this ResourceCollection collection, int resourceId)
        {
            collection.Add<ModuleAPermission>(resourceId);
            return collection;
        }
    }
}
