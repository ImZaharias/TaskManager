using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Tasks;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public EditModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    [FromRoute]
    public int Id { get; set; }

    public class InputModel
    {
        [Required, MaxLength(160)]
        public string Title { get; set; } = "";

        [MaxLength(2000)]
        public string? Description { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _current.UserId!.Value;

        var task = await _db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        Input = new InputModel { Title = task.Title, Description = task.Description };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = _current.UserId!.Value;

        var task = await _db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        task.Title = Input.Title;
        task.Description = Input.Description;
        await _db.SaveChangesAsync();

        return RedirectToPage("Details", new { id = Id });
    }
}