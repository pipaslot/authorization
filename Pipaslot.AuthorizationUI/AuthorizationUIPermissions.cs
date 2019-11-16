using Pipaslot.Authorization.Attributes;

namespace Pipaslot.AuthorizationUI
{
    public enum AuthorizationUIPermissions
    {
        [StaticPermission("Access Security UI")]
        Access = 0
    }
}
