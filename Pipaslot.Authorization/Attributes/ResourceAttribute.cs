using System;

namespace Pipaslot.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ResourceAttribute : Attribute
    {
        public ResourceAttribute(string name, string description = null)
        {
            this.Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}