using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public class Project
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    public int OwnerUserId { get; set; }
    public AppUser OwnerUser { get; set; } = null!;
}