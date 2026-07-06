using System.Security.Claims;

namespace TaskManager.Services.Auth;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    public int? UserId
    {
        get
        {
            var idStr = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : null;
        }
    }

    public string? DisplayName => _http.HttpContext?.User?.Identity?.Name;

    public bool IsAdmin =>
        string.Equals(_http.HttpContext?.User?.FindFirstValue("isAdmin"), "true", StringComparison.OrdinalIgnoreCase);
}