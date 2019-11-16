using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Pipaslot.Authorization.Models;

namespace Pipaslot.Authorization
{
    /// <summary>
    /// Provide current user claims principal. In final implementation this can be only using IHttpContextAccessor
    /// </summary>
    public interface IClaimsPrincipalProvider
    {
        ClaimsPrincipal GetClaimsPrincipal();
        
        Claim RoleToClaim(IRole role);
        IRole ClaimToRole(Claim claim);
    }
}
