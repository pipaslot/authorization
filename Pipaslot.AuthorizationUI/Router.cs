using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pipaslot.Authorization;
using Pipaslot.AuthorizationUI.ActionAbstraction;
using Pipaslot.AuthorizationUI.Actions;

namespace Pipaslot.AuthorizationUI
{
    internal class Router<TKey>
    {
        private readonly HttpRequest _request;
        private readonly IUser<TKey> _user;
        private readonly string _routePrefix;

        public Router(HttpRequest request, string routePrefix, IUser<TKey> user)
        {
            _request = request;
            _user = user;
            _routePrefix = "/" + routePrefix.TrimStart('/');
        }

        public IAction ResolveAction()
        {
            //Assets
            if (MatchFile("assets", ".js"))
            {
                var assetName = _request.Path.Value.Substring($"{_routePrefix}/assets".Length);
                return new JavascriptAction(assetName);
            }
            if (MatchFile("assets", ".css"))
            {
                var assetName = _request.Path.Value.Substring($"{_routePrefix}/assets".Length);
                return new StyleAction(assetName);
            }

            //API
            if (MatchAndAuthorize("api/roles"))
            {
                return new RoleJsonAction<TKey>();
            }
            if (MatchAndAuthorize("api/resources"))
            {
                _request.Query.TryGetValue("role", out var role);
                return new ResourcesJsonAction<TKey>(ChangeType<TKey>(role));
            }
            if (MatchAndAuthorize("api/resource-instances"))
            {
                _request.Query.TryGetValue("role", out var role);
                _request.Query.TryGetValue("resourceId", out var resourceId);
                return new ResourcesInstancesJsonAction<TKey>(ChangeType<TKey>(role), int.Parse(resourceId));
            }
            if (MatchAndAuthorize("api/privilege"))
            {
                _request.Query.TryGetValue("role", out var role);
                _request.Query.TryGetValue("resourceId", out var resourceId);
                _request.Query.TryGetValue("permissionId", out var permissionId);
                _request.Query.TryGetValue("instanceId", out var instanceId);
                _request.Query.TryGetValue("isAllowed", out var isAllowedString);
                bool? isAllowed = null;
                if (isAllowedString.Equals("true")) isAllowed = true;
                else if (isAllowedString.Equals("false")) isAllowed = false;
                var resourceIdInt = int.Parse(resourceId);
                var permissionIdInt = int.Parse(permissionId);
                return new PrivilegeJsonAction<TKey>(ChangeType<TKey>(role), resourceIdInt, permissionIdInt, instanceId, isAllowed);
            }
            
            return new TemplateAction(_routePrefix, "index");
        }

        private bool MatchAndAuthorize(string path, string method = "GET")
        {
            var expectedPath = $"{_routePrefix}/{path}";
            var matches = _request.Path.Value.StartsWith(expectedPath) &&
                          string.Equals(_request.Method, method, StringComparison.CurrentCultureIgnoreCase);
            if (matches)
            {
                _user.CheckPermission(AuthorizationUIPermissions.Access);
            }
            
            return matches;
        }

        private bool MatchFile(string path, string suffix)
        {
            var expectedPath = $"{_routePrefix}/{path}";
            return _request.Path.Value.StartsWith(expectedPath) && _request.Path.Value.EndsWith(suffix);
        }

        private T ChangeType<T>(StringValues obj)
        {
            var value = obj.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}