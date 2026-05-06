using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<Result<TaskResponseDto>> CreateAsync(CreateTaskDto dto);
    Task<Result<IEnumerable<TaskResponseDto>>> GetAllAsync(TaskStatus? status = null);
    Task<Result> ChangeStatusAsync(Guid taskId, ChangeTaskStatusDto dto);
    Task<Result<IEnumerable<TaskResponseDto>>> GetByJsonPriorityAsync(string priority);
    Task<Result<TaskResponseDto>> UpdateTaskAsync(Guid id, UpdateTaskDto dto);
    Task<Result<TaskResponseDto>> UpdateAdditionalInfoAsync(Guid id, UpdateAdditionalInfoDto dto);
}
