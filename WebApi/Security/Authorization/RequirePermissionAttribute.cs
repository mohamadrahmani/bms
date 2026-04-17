using Microsoft.AspNetCore.Authorization;

namespace WebApi.Security.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = permission;
    }
}
