using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public class Comment
{
    public int Id { get; set; }

    [Required, MaxLength(2000)]
    public string Body { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;

    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
}