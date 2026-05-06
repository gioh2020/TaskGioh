using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(Entities.TaskItem task);
    Task<IEnumerable<Entities.TaskItem>> GetAllAsync();
    Task<IEnumerable<Entities.TaskItem>> GetByStatusAsync(TaskStatus status);
    Task<Entities.TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.TaskItem>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Entities.TaskItem>> GetByJsonPriorityAsync(string priority);
}
