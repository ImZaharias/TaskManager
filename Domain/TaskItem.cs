using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Domain;

public class TaskItem
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? AssignedToUserId { get; set; }
    public AppUser? AssignedToUser { get; set; }

    public List<Comment> Comments { get; set; } = new();

    public List<TaskTag> TaskTags { get; set; } = new();
}