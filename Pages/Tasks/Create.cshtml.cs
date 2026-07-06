using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Tasks;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public CreateModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public class InputModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required, MaxLength(160)]
        public string Title { get; set; } = "";

        [MaxLength(2000)]
        public string? Description { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> ProjectOptions { get; set; } = new();

    public async Task OnGetAsync(int? projectId)
    {
        var userId = _current.UserId!.Value;

        var projectsQuery = _db.Projects.AsNoTracking();

        if (!_current.IsAdmin)
            projectsQuery = projectsQuery.Where(p => p.OwnerUserId == userId);

        var projects = await projectsQuery
            .OrderBy(p => p.Name)
            .ToListAsync();

        ProjectOptions = projects.Select(p => new SelectListItem(p.Name, p.Id.ToString())).ToList();

        if (projectId.HasValue)
            Input.ProjectId = projectId.Value;
        else if (projects.Count > 0)
            Input.ProjectId = projects[0].Id;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(Input.ProjectId);
            return Page();
        }

        var userId = _current.UserId!.Value;

        var ok = await _db.Projects.AnyAsync(p => p.Id == Input.ProjectId && (_current.IsAdmin || p.OwnerUserId == userId));
        if (!ok) return Forbid();

        var task = new TaskItem
        {
            ProjectId = Input.ProjectId,
            Title = Input.Title,
            Description = Input.Description
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return RedirectToPage("Details", new { id = task.Id });
    }
}