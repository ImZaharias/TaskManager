using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Services.Business;

public class TaskWorkflowService
{
    public bool CanTransition(TaskStatus from, TaskStatus to) =>
        (from, to) switch
        {
            (TaskStatus.ToDo, TaskStatus.InProgress) => true,
            (TaskStatus.ToDo, TaskStatus.Blocked) => true,

            (TaskStatus.InProgress, TaskStatus.Done) => true,
            (TaskStatus.InProgress, TaskStatus.Blocked) => true,

            (TaskStatus.Blocked, TaskStatus.InProgress) => true,

            _ => false
        };
}