using System;
using System.Collections.Generic;

namespace Pipaslot.Authorization
{
    /// <summary>
    /// Provides conversion from user claims principal
    /// </summary>
    public interface IIdentityProvider<out TUserId>
    {
        /// <summary>
        /// Provide user id which can be used as unique identifier
        /// </summary>
        /// <returns></returns>
        TUserId GetUserId();

        /// <summary>
        /// User is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Return all user's custom roles
        /// </summary>
        /// <returns></returns>
        List<string> GetRoles();
    }
}
