using Enums;

namespace Presentation_Layer.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]

public class CheckPermissionAttribute : Attribute
{
    public Permission Permission { get; }

    public CheckPermissionAttribute(Permission permission)
    {
        Permission = permission;
    }

}
