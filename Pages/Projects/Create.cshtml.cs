using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Data;
using TaskManager.Domain;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Projects;

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
        [Required, MaxLength(120)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = _current.UserId!.Value;

        var project = new Project
        {
            Name = Input.Name,
            Description = Input.Description,
            OwnerUserId = userId
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return RedirectToPage("Details", new { id = project.Id });
    }
}