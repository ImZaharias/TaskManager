using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Projects;

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
        [Required, MaxLength(120)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _current.UserId!.Value;

        var project = await _db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == Id && (_current.IsAdmin || p.OwnerUserId == userId));

        if (project is null) return NotFound();

        Input = new InputModel
        {
            Name = project.Name,
            Description = project.Description
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = _current.UserId!.Value;

        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == Id && (_current.IsAdmin || p.OwnerUserId == userId));

        if (project is null) return NotFound();

        project.Name = Input.Name;
        project.Description = Input.Description;

        await _db.SaveChangesAsync();

        return RedirectToPage("Details", new { id = Id });
    }
}