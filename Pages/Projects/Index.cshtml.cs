using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Projects;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public IndexModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public List<Project> Projects { get; set; } = new();
    public string? Q { get; set; }

    public async Task OnGetAsync(string? q)
    {
        Q = q;
        var userId = _current.UserId!.Value;

        var query = _db.Projects.AsNoTracking();

        if (!_current.IsAdmin)
            query = query.Where(p => p.OwnerUserId == userId);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Name.Contains(q));

        Projects = await query.OrderBy(p => p.Name).ToListAsync();
    }
}