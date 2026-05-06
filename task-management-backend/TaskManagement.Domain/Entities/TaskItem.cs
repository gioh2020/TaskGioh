using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; private set; } 
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public Guid AssignedUserId { get; private set; }
    public User? AssignedUser { get; private set; }
    public string? AdditionalInfo { get; private set; }

    private TaskItem() { }

    public static TaskItem Create(string title, string? description, Guid assignedUserId, string? additionalInfo = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la tarea es obligatorio.", nameof(title));

        if (assignedUserId == Guid.Empty)
            throw new ArgumentException("La tarea debe tener un usuario asignado.", nameof(assignedUserId));

        return new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Title = title.Trim(),
            Description = description?.Trim(),
            Status = TaskStatus.Pending,
            AssignedUserId = assignedUserId,
            AdditionalInfo = additionalInfo
        };
    }

    public bool CanTransitionTo(TaskStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (TaskStatus.Pending, TaskStatus.InProgress) => true,
            (TaskStatus.InProgress, TaskStatus.Done)    => true,
            _                                           => false
        };
    }

    public void ChangeStatus(TaskStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException(
                $"No se puede cambiar el estado de '{Status}' a '{newStatus}'. " +
                $"Transiciones permitidas: Pending→InProgress, InProgress→Done.");

        Status = newStatus;
    }

    public void UpdateAdditionalInfo(string? additionalInfo)
    {
        AdditionalInfo = additionalInfo;
    }

    public void UpdateDetails(string title, string? description)
    {
        if (Status == TaskStatus.Done)
            throw new InvalidOperationException("No se puede editar el título o la descripción de una tarea que ya ha sido completada.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título de la tarea es obligatorio.", nameof(title));

        Title = title.Trim();
        Description = description?.Trim();
    }
}
