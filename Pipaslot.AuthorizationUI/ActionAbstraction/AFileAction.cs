using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.AuthorizationUI.ActionAbstraction
{
    abstract class AFileAction : IAction
    {
        protected string ReadResource(string relativePath)
        {
            var path = relativePath.Replace('\\', '.').Replace('/', '.').Replace("..", ".").TrimStart('.');
            var assembly = GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream($"Pipaslot.AuthorizationUI.{path}"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public abstract Task ExecuteAsync(HttpContext context, IServiceProvider services);
    }
}
