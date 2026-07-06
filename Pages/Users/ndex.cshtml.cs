using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskManager.Services.Auth;

namespace TaskManager.Pages.Users;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;

    public IndexModel(AppDbContext db, CurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public List<AppUser> Users { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!_current.IsAdmin)
            return Forbid();

        Users = await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.DisplayName)
            .ToListAsync();

        return Page();
    }
}