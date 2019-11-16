using System;

namespace Pipaslot.Authorization.Attributes
{
    /// <summary>
    /// Permission applicable for whole resource regardless if it is instance or not
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class StaticPermissionAttribute : Attribute
    {
        public StaticPermissionAttribute(string name, string description = null)
        {
            this.Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}