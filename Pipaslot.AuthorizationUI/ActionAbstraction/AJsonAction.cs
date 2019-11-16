using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pipaslot.AuthorizationUI.ActionAbstraction
{
    abstract class AJsonAction : IAction
    {
        public async Task ExecuteAsync(HttpContext context, IServiceProvider services)
        {
            var response = context.Response;

            response.ContentType = "application/json";
            var data = GetData(context, services);
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var json = JsonConvert.SerializeObject(data, serializerSettings);
            await response.WriteAsync(json);
        }

        protected abstract object GetData(HttpContext context, IServiceProvider services);
    }
}
