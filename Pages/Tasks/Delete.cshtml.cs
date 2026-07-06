using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Tasks;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public DeleteModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    [FromRoute]
    public int Id { get; set; }

    public TaskItem? Task { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _current.UserId!.Value;

        Task = await _db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (Task is null) return NotFound();

        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = _current.UserId!.Value;

        var task = await _db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}