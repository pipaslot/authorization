using System;
using Pipaslot.Authorization.Attributes;

namespace Demo.ModuleA
{
    [Resource("Module A", "Custom module description")]
    public enum ModuleAPermission
    {
        [StaticPermission("User can create module A")]
        Create = 1,
        [InstancePermission("User can view module A")]
        View = 2,
        [InstancePermission("User can edit module A")]
        Update = 3,
        [InstancePermission("User can delete module A")]
        Delete = 4,

        [StaticPermission("Hack it !!!!", "Super cool translatable description for not so smart users")]
        DoItAsGood = 6,
    }
}
