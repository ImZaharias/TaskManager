using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public class AppUser
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string DisplayName { get; set; } = "";

    [Required, MaxLength(120)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    public bool IsAdmin { get; set; }
}