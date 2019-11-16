using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.AuthorizationUI
{
    internal abstract class APage
    {
        internal async Task Run(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            var rendered = Render();
            var contentBytes = Encoding.ASCII.GetBytes(rendered.Content);
            context.Response.ContentType = rendered.ContentType;
            context.Response.ContentLength = contentBytes.Length;

            using (var stream = context.Response.Body)
            {
                await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
                await stream.FlushAsync();
            }
        }

        protected abstract (string Content, string ContentType) Render();
    }
}
