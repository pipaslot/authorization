﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pipaslot.Authorization;

namespace Pipaslot.AuthorizationUI
{
    public class AuthorizationUIMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthorizationUIOptions _options;

        public AuthorizationUIMiddleware(RequestDelegate next, AuthorizationUIOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context, IServiceProvider services)
        {
            if (context.Request.Path.Value.StartsWith($"/{_options.RoutePrefix}"))
            {
                try
                {
                    var user = (IUser) services.GetService(typeof(IUser));
                    var router = new Router(context.Request, _options.RoutePrefix, user);
                    var action = router.ResolveAction();
                    await action.ExecuteAsync(context, services);
                }
                catch (AuthorizationException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
