using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Projects;

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

    public Project? Project { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _current.UserId!.Value;

        Project = await _db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == Id && (_current.IsAdmin || p.OwnerUserId == userId));

        if (Project is null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = _current.UserId!.Value;

        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == Id && (_current.IsAdmin || p.OwnerUserId == userId));

        if (project is null) return NotFound();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}