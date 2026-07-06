using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.Services.Auth;

namespace TaskManager.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IServiceProvider sp)
    {
        var hasher = sp.GetRequiredService<PasswordHasher>();

        // USERS (upsert by Email)
        var admin = await EnsureUserAsync(db, hasher, "Admin", "admin@local", "Admin123!", isAdmin: true);
        var zach = await EnsureUserAsync(db, hasher, "Zach", "zach@local", "Zach123!", isAdmin: false);
        var dimitris = await EnsureUserAsync(db, hasher, "Dimitris", "dimitris@local", "Dimitris123!", isAdmin: false);

        // Each user gets their own project
        var zachProject = await EnsureProjectAsync(db, zach.Id, "MiniCRM Task Managerv1", "MiniCRM Task Managerv3");
        var dimProject = await EnsureProjectAsync(db, dimitris.Id, "MiniCRM Task Managerv2", "MiniCRM Task Managerv4");

        // Seed tasks per project if none exist (avoid duplicates)
        await EnsureTasksForProjectAsync(db, zachProject.Id, zach.Id);
        await EnsureTasksForProjectAsync(db, dimProject.Id, dimitris.Id);
    }

    private static async Task<AppUser> EnsureUserAsync(
        AppDbContext db,
        PasswordHasher hasher,
        string displayName,
        string email,
        string password,
        bool isAdmin)
    {
        var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existing != null) return existing;

        var user = new AppUser
        {
            DisplayName = displayName,
            Email = email,
            IsAdmin = isAdmin,
            PasswordHash = hasher.Hash(password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    private static async Task<Project> EnsureProjectAsync(
        AppDbContext db,
        int ownerUserId,
        string name,
        string? description)
    {
        var existing = await db.Projects.FirstOrDefaultAsync(p => p.OwnerUserId == ownerUserId && p.Name == name);
        if (existing != null) return existing;

        var project = new Project
        {
            OwnerUserId = ownerUserId,
            Name = name,
            Description = description
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return project;
    }

    private static async Task EnsureTasksForProjectAsync(AppDbContext db, int projectId, int assigneeUserId)
    {
        if (await db.Tasks.AnyAsync(t => t.ProjectId == projectId)) return;

        db.Tasks.AddRange(
            new TaskItem
            {
                ProjectId = projectId,
                Title = "Project 1",
                Description = "Project 1 description",
                Status = TaskStatus.InProgress,
                AssignedToUserId = assigneeUserId
            },
            new TaskItem
            {
                ProjectId = projectId,
                Title = "Project 2",
                Description = "Project 2 description",
                Status = TaskStatus.ToDo
            },
            new TaskItem
            {
                ProjectId = projectId,
                Title = "Project 3",
                Status = TaskStatus.Blocked
            }
        );

        await db.SaveChangesAsync();

        var firstTaskId = await db.Tasks
            .Where(t => t.ProjectId == projectId)
            .Select(t => t.Id)
            .FirstAsync();

        if (!await db.Comments.AnyAsync(c => c.TaskItemId == firstTaskId))
        {
            db.Comments.Add(new Comment
            {
                TaskItemId = firstTaskId,
                CreatedByUserId = assigneeUserId,
                Body = "Task is Updated."
            });

            await db.SaveChangesAsync();
        }
        if (!await db.Tags.AnyAsync())
        {
            db.Tags.AddRange(
                new Tag { Name = "Backend" },
                new Tag { Name = "Frontend" },
                new Tag { Name = "Bug" },
                new Tag { Name = "Urgent" }
            );
            await db.SaveChangesAsync();
        }
    }
}