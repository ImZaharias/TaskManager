using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Projects;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public DetailsModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public Project? Project { get; set; }
    public Dictionary<TaskStatus, List<TaskItem>> TasksByStatus { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = _current.UserId!.Value;

        // 1) Load Project + access check (admin OR owner)
        Project = await _db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && (_current.IsAdmin || p.OwnerUserId == userId));

        if (Project is null) return NotFound();

        // 2) Load Tasks for this project
        var tasks = await _db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == id)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync();

        TasksByStatus = tasks
            .GroupBy(t => t.Status)
            .ToDictionary(g => g.Key, g => g.ToList());

        return Page();
    }
}