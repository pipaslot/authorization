using Pipaslot.Authorization.Attributes;

namespace Pipaslot.AuthorizationUI
{
    [Resource("Permission management")]
    public enum AuthorizationUIPermissions
    {
        [StaticPermission("Access Admin UI")]
        Access = 0
    }
}
