using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.AuthorizationUI.ActionAbstraction
{
    interface IAction
    {
        Task ExecuteAsync(HttpContext context, IServiceProvider services);
    }
}
