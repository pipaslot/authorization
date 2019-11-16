using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pipaslot.AuthorizationUI.ActionAbstraction
{
    class TemplateAction : AFileAction
    {
        private readonly Dictionary<string, object> _templateParameters;
        public string RoutePrefix { get; set; }
        public string PageName { get; set; }

        public TemplateAction(string routePrefix, string pageName, Dictionary<string, object>templateParameters = null)
        {
            _templateParameters = templateParameters ?? new Dictionary<string, object>();
            RoutePrefix = routePrefix;
            PageName = pageName;
        }

        public override async Task ExecuteAsync(HttpContext context, IServiceProvider services)
        {
            var response = context.Response;
            response.ContentType = "text/html";
            var layout = ReadResource("Templates.layout.html");
            var body = ReadResource($"Templates.{PageName}.html");
            var token = "";
            if (context.Request.Query.TryGetValue("authentication", out var tokenValue))
            {
                token = tokenValue.ToString();
            }
            var html = layout
                .Replace("{{pageBody}}", body)
                .Replace("{{routePrefix}}",RoutePrefix)
                .Replace("{{authenticationToken}}",token);
            foreach (var parameter in _templateParameters)
            {
                html = html.Replace("{{"+ parameter.Key+"}}", parameter.Value.ToString());
            }
            
            await response.WriteAsync(html);
        }
    }
}
