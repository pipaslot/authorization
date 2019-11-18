using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization.Web;

namespace Demo.App.Models
{
    public class WindowsIdentityProvider : IdentityProvider<long>
    {
        public WindowsIdentityProvider(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }
    }
}
