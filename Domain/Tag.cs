using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public class Tag
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    public List<TaskTag> TaskTags { get; set; } = new();
}