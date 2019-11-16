using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.AuthorizationUI.ActionAbstraction
{
    class StyleAction : AFileAction
    {
        public string FileName { get; set; }

        public StyleAction(string fileName)
        {
            FileName = fileName;
        }

        public override async Task ExecuteAsync(HttpContext context, IServiceProvider services)
        {
            context.Response.ContentType = "text/css";
            var content = ReadResource($"Assets.{FileName}");

            await context.Response.WriteAsync(content);
        }

    }
}
