using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.Authorization.Web
{
    public class IdentityProvider<TUserId> : IdentityProviderBase<TUserId>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        protected override ClaimsPrincipal GetClaimPrincipal()
        {
            return _contextAccessor.HttpContext.User;
        }

        protected override TUserId WindowsUserNameToId(string name)
        {
            throw new NotImplementedException($"Username conversion for Windows identity is not implemented. Override method {nameof(WindowsUserNameToId)} to add windows identity conversion support");
        }
    }
}
