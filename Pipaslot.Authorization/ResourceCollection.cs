using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pipaslot.Authorization
{
    /// <summary>
    /// Collect permission for all resources
    /// </summary>
    public class ResourceCollection : IReadOnlyCollection<ResourceDefinition>
    {
        private readonly List<ResourceDefinition> _resources = new List<ResourceDefinition>();

        public ResourceCollection(IEnumerable<ResourceDefinition> definitions = null)
        {
            if (definitions != null)
            {
                foreach (var definition in definitions)
                {
                    Add(definition);
                }
            }
        }

        public ResourceCollection Add<TConvertible>(int resourceUniqueId) where TConvertible : IConvertible
        {
            var permissionEnumType = typeof(TConvertible);
            var definition = new ResourceDefinition(permissionEnumType, resourceUniqueId);
            return Add(definition);
        }

        public ResourceCollection Add<TConvertible, TInstanceProvider>(int resourceUniqueId)
            where TConvertible : IConvertible
            where TInstanceProvider : IResourceInstanceProvider
        {
            var permissionEnumType = typeof(TConvertible);
            var instanceProviderType = typeof(TInstanceProvider);
            var definition = new ResourceDefinition(permissionEnumType, resourceUniqueId, instanceProviderType);
            return Add(definition);
        }

        private ResourceCollection Add(ResourceDefinition definition)
        {
            if (_resources.Any(r => r.PermissionEnumType == definition.PermissionEnumType))
            {
                throw new Exception(
                    $"Resource for enum {definition.PermissionEnumType} is already registered and can not be overriden");
            }

            CheckCollisions(definition);
            CheckDuplicateKeys(definition.PermissionEnumType);
            _resources.Add(definition);
            return this;
        }

        private void CheckCollisions(ResourceDefinition definition)
        {
            var rangeCollision = _resources.FirstOrDefault(r => r.ResourceId == definition.ResourceId);
            if (rangeCollision != null)
            {
                throw new Exception(
                    $"Resource for enum {definition.PermissionEnumType} with ID {definition.ResourceId} is in collision with {rangeCollision.PermissionEnumType} containing the same resource ID");
            }
        }

        private void CheckDuplicateKeys(Type definitionPermissionEnumType)
        {
            var values = Enum.GetValues(definitionPermissionEnumType);
            var keys = new List<int>(values.Length);
            foreach (var value in values)
            {
                keys.Add((int)value);
            }

            var duplicates = keys.GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();
            if (duplicates.Any())
            {
                throw new Exception($"Enum {definitionPermissionEnumType} contains duplicated keys {string.Join(", ", duplicates)}");
            }
        }

        public IEnumerator<ResourceDefinition> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _resources.Count;
    }
}
