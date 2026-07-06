using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Domain;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.Services.Auth;
using TaskManager.Services.Business;

namespace TaskManager.Pages.Tasks;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _current;
    private readonly TaskWorkflowService _workflow;

    public DetailsModel(AppDbContext db, CurrentUserService current, TaskWorkflowService workflow)
    {
        _db = db;
        _current = current;
        _workflow = workflow;
    }

    [FromRoute]
    public int Id { get; set; }

    public TaskItem? Task { get; set; }
    public string? Error { get; set; }

    public List<string> StatusOptions { get; } =
        Enum.GetValues<TaskStatus>().Select(s => s.ToString()).ToList();

    public List<AppUser> UserOptions { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();

    public List<Tag> AllTags { get; set; } = new();
    public List<Tag> CurrentTags { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _current.UserId!.Value;

        Task = await _db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (Task is null) return NotFound();

        AllTags = await _db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();

        CurrentTags = await _db.TaskTags
            .Where(tt => tt.TaskItemId == Id)
            .Include(tt => tt.Tag)
            .Select(tt => tt.Tag)
            .ToListAsync();

        UserOptions = await _db.Users.AsNoTracking().OrderBy(u => u.DisplayName).ToListAsync();

        Comments = await _db.Comments
            .AsNoTracking()
            .Include(c => c.CreatedByUser)
            .Where(c => c.TaskItemId == Id)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(string newStatus)
    {
        var userId = _current.UserId!.Value;

        var task = await _db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        if (!Enum.TryParse<TaskStatus>(newStatus, out var to))
        {
            Error = "Invalid status.";
            return await OnGetAsync();
        }

        if (!_workflow.CanTransition(task.Status, to))
        {
            Error = $"Transition not allowed: {task.Status} → {to}";
            return await OnGetAsync();
        }

        task.Status = to;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostAssignAsync(int? assignedToUserId)
    {
        var userId = _current.UserId!.Value;

        var task = await _db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        if (assignedToUserId.HasValue)
        {
            var exists = await _db.Users.AnyAsync(u => u.Id == assignedToUserId.Value);
            if (!exists)
            {
                Error = "User not found.";
                return await OnGetAsync();
            }
        }

        task.AssignedToUserId = assignedToUserId;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostAddCommentAsync(string body)
    {
        var userId = _current.UserId!.Value;

        // Validate access to task
        var task = await _db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        if (string.IsNullOrWhiteSpace(body))
        {
            Error = "Comment cannot be empty.";
            return await OnGetAsync();
        }

        if (body.Length > 2000)
        {
            Error = "Comment too long (max 2000 chars).";
            return await OnGetAsync();
        }

        _db.Comments.Add(new Comment
        {
            TaskItemId = Id,
            CreatedByUserId = userId,
            Body = body.Trim()
        });

        await _db.SaveChangesAsync();
        return RedirectToPage(new { id = Id });
    }
    
    public async Task<IActionResult> OnPostAddTagAsync(int tagId)
    {
        var userId = _current.UserId!.Value;

        var task = await _db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == Id && (_current.IsAdmin || t.Project.OwnerUserId == userId));

        if (task is null) return NotFound();

        if (!await _db.TaskTags.AnyAsync(tt => tt.TaskItemId == Id && tt.TagId == tagId))
        {
            _db.TaskTags.Add(new TaskTag
            {
                TaskItemId = Id,
                TagId = tagId
            });

            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostRemoveTagAsync(int tagId)
    {
        var link = await _db.TaskTags
            .FirstOrDefaultAsync(tt => tt.TaskItemId == Id && tt.TagId == tagId);

        if (link != null)
        {
            _db.TaskTags.Remove(link);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = Id });
    }
}
