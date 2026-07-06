using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.Services.Auth;

namespace TaskManager.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public IndexModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public Dictionary<TaskStatus, int> Counts { get; set; } = new();
    public List<TaskItem> RecentTasks { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = _current.UserId!.Value;

        // All tasks owned by me (via my projects)
        var baseQuery = _db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Where(t => t.Project.OwnerUserId == userId);

        // counts by status
        var grouped = await baseQuery
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        Counts = grouped.ToDictionary(x => x.Status, x => x.Count);

        // recent tasks
        RecentTasks = await baseQuery
            .OrderByDescending(t => t.CreatedAtUtc)
            .Take(10)
            .ToListAsync();
    }

    public int Count(TaskStatus status) => Counts.TryGetValue(status, out var c) ? c : 0;
}