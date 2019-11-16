using Pipaslot.Authorization;

namespace Demo.ModuleB
{
    public static class ResourceCollectionExtensions
    {
        public static ResourceCollection AddModuleB(this ResourceCollection collection, int resourceId)
        {
            collection.Add<ModuleBPermission>(resourceId);
            return collection;
        }
    }
}
