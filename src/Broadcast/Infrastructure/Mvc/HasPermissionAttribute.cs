using System.Threading.Tasks;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Broadcast.Infrastructure.Mvc
{
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(StandardPermission permission) : base(typeof(PermissionRequirementFilter))
        {
            Arguments = new object[] {permission.ToString()};
        }

        public class PermissionRequirementFilter : IAsyncAuthorizationFilter
        {
            private readonly string _permissionName;
            private readonly IPermissionService _permissionService;

            public PermissionRequirementFilter(string permissionName, IPermissionService permissionService)
            {
                _permissionName = permissionName;
                _permissionService = permissionService;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                if (!await _permissionService.AuthorizeAsync(_permissionName))
                    context.Result = new ForbidResult();
            }
        }
    }
}