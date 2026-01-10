using System.Data;

namespace Presentation_Layer.Authorization;

public class AuthorizeHelper
{
    public IHttpContextAccessor _httpContextAccessor { get; }
    public HttpContext HttpContext => _httpContextAccessor.HttpContext;


    public AuthorizeHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<int> GetUserPermissions()
    {
        if (HttpContext?.User == null)
        {
            return new List<int>();
        }

        return HttpContext.User.FindAll("Permission")
            .Select(c => int.TryParse(c.Value, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
    }

}
