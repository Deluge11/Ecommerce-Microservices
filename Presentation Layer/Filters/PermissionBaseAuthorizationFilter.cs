using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation_Layer.Authorization;

namespace Presentation_Layer.Filters;

public class PermissionBaseAuthorizationFilter : IAuthorizationFilter
{

    public AuthorizeHelper AuthorizeHelper { get; }

    public PermissionBaseAuthorizationFilter(AuthorizeHelper authorizeHelper)
    {
        AuthorizeHelper = authorizeHelper;
    }


    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
           .OfType<AllowAnonymousAttribute>()
           .Any();

        if (allowAnonymous) return;

        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<CheckPermissionAttribute>()
            .FirstOrDefault();

        if (attribute == null) return;

        var userPermissions = AuthorizeHelper.GetUserPermissions();
        if (!userPermissions.Contains((int)attribute.Permission))
        {
            context.Result = new ForbidResult();
        }
    }
}
