using System;
using System.Collections.Generic;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    /// <summary>
    /// Provides conversion from user claims principal
    /// </summary>
    public interface IIdentityProvider<out TUserId>
    {
        TUserId GetUserId();

        bool IsAuthenticated { get; }

        List<string> GetRoles();
    }
}
