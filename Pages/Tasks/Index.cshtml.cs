using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Tasks;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public IndexModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public List<TaskItem> Tasks { get; set; } = new();
    public string? Q { get; set; }
    public string? Status { get; set; }

    public List<SelectListItem> StatusOptions { get; } =
        Enum.GetValues<TaskStatus>()
            .Select(s => new SelectListItem(s.ToString(), s.ToString()))
            .ToList();

    public async Task OnGetAsync(string? q, string? status)
    {
        Q = q;
        Status = status;

        var userId = _current.UserId!.Value;

        IQueryable<TaskItem> query = _db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser);

        if (!_current.IsAdmin)
            query = query.Where(t => t.Project.OwnerUserId == userId);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(t => t.Title.Contains(q));

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TaskStatus>(status, out var st))
            query = query.Where(t => t.Status == st);

        Tasks = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync();
    }
}