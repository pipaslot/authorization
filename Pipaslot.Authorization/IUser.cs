using System;

namespace Pipaslot.Authorization
{
    public interface IUser<out TUserId> : IUser
    {
        /// <summary>
        /// User identifier
        /// </summary>
        TUserId Id { get; }
    }

    /// <summary>
    /// Singleton service providing user authorization operations
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Flag if user was successfully authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Check if User has got required permission, if not, then Exception is thrown.
        /// </summary>
        /// <exception cref="AuthorizationException"></exception>
        void CheckPermission(IConvertible permissionEnum);

        /// <summary>
        /// Check if User has got required permission, if not, then Exception is thrown.
        /// </summary>
        /// <exception cref="AuthorizationException"></exception>
        void CheckPermission<TInstanceKey>(IConvertible permissionEnum, TInstanceKey instanceKey);

        /// <summary>
        /// Check if user has permission
        /// </summary>
        /// <param name="permissionEnum">Requested permission</param>
        /// <returns></returns>
        bool IsAllowed(IConvertible permissionEnum);

        /// <summary>
        /// Check if user has permission
        /// </summary>
        /// <param name="permissionEnum">Requested permission</param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        bool IsAllowed<TInstanceKey>(IConvertible permissionEnum, TInstanceKey instanceKey);

    }
}