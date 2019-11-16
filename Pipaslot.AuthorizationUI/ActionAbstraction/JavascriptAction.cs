using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.AuthorizationUI.ActionAbstraction
{
    class JavascriptAction : AFileAction
    {
        public string FileName { get; set; }

        public JavascriptAction(string fileName)
        {
            FileName = fileName;
        }

        public override async Task ExecuteAsync(HttpContext context, IServiceProvider services)
        {
            context.Response.ContentType = "application/javascript";
            var content = ReadResource($"Assets.{FileName}");

            await context.Response.WriteAsync(content);
        }

    }
}
