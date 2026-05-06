using System.Text.Json;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskResponseDto>> CreateAsync(CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<TaskResponseDto>.Failure("El título de la tarea es obligatorio.", 400);

        if (dto.AssignedUserId == Guid.Empty)
            return Result<TaskResponseDto>.Failure("Debe especificar un usuario asignado.", 400);

        if (!string.IsNullOrEmpty(dto.AdditionalInfo))
        {
            try { JsonDocument.Parse(dto.AdditionalInfo); }
            catch (JsonException)
            {
                return Result<TaskResponseDto>.Failure("El campo 'additionalInfo' debe ser un JSON válido.", 400);
            }
        }

        var user = await _unitOfWork.Users.GetByIdAsync(dto.AssignedUserId);
        if (user is null)
            return Result<TaskResponseDto>.Failure($"No existe un usuario con el Id '{dto.AssignedUserId}'.", 404);

        var task = Domain.Entities.TaskItem.Create(dto.Title, dto.Description, dto.AssignedUserId, dto.AdditionalInfo);
        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return Result<TaskResponseDto>.Success(MapToDto(task, user));
    }

    public async Task<Result<IEnumerable<TaskResponseDto>>> GetAllAsync(TaskStatus? status = null)
    {
        var tasks = status.HasValue
            ? await _unitOfWork.Tasks.GetByStatusAsync(status.Value)
            : await _unitOfWork.Tasks.GetAllAsync();

        return Result<IEnumerable<TaskResponseDto>>.Success(tasks.Select(t => MapToDto(t, t.AssignedUser!)));
    }

    public async Task<Result<TaskResponseDto>> UpdateTaskAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null)
            return Result<TaskResponseDto>.Failure("Tarea no encontrada.", 404);

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<TaskResponseDto>.Failure("El título de la tarea es obligatorio.", 400);

        try
        {
            task.UpdateDetails(dto.Title, dto.Description);
        }
        catch (InvalidOperationException ex)
        {
            return Result<TaskResponseDto>.Failure(ex.Message, 400);
        }

        await _unitOfWork.SaveChangesAsync();
        
        var user = await _unitOfWork.Users.GetByIdAsync(task.AssignedUserId);
        return Result<TaskResponseDto>.Success(MapToDto(task, user!));
    }

    public async Task<Result> ChangeStatusAsync(Guid taskId, ChangeTaskStatusDto dto)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        if (task is null)
            return Result.Failure($"No existe una tarea con el Id '{taskId}'.", 404);

        try
        {
            task.ChangeStatus(dto.NewStatus);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message, 422);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<IEnumerable<TaskResponseDto>>> GetByJsonPriorityAsync(string priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
            return Result<IEnumerable<TaskResponseDto>>.Failure("Debe especificar una prioridad.", 400);

        var tasks = await _unitOfWork.Tasks.GetByJsonPriorityAsync(priority);
        return Result<IEnumerable<TaskResponseDto>>.Success(tasks.Select(t => MapToDto(t, t.AssignedUser!)));
    }

    public async Task<Result<TaskResponseDto>> UpdateAdditionalInfoAsync(Guid id, UpdateAdditionalInfoDto dto)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null)
            return Result<TaskResponseDto>.Failure("Tarea no encontrada.", 404);

        // Parse existing JSON or create new object
        var jsonObj = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(task.AdditionalInfo))
        {
            try
            {
                var existing = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(task.AdditionalInfo);
                if (existing != null)
                {
                    foreach (var kvp in existing)
                        jsonObj[kvp.Key] = kvp.Value;
                }
            }
            catch (JsonException) { /* ignore malformed JSON, start fresh */ }
        }

        // Update specific field
        if (dto.EstimatedEndDate != null)
            jsonObj["estimatedEndDate"] = dto.EstimatedEndDate;
        else
            jsonObj.Remove("estimatedEndDate");

        var updatedJson = JsonSerializer.Serialize(jsonObj);
        task.UpdateAdditionalInfo(updatedJson);

        await _unitOfWork.SaveChangesAsync();

        var user = await _unitOfWork.Users.GetByIdAsync(task.AssignedUserId);
        return Result<TaskResponseDto>.Success(MapToDto(task, user!));
    }

    private TaskResponseDto MapToDto(Domain.Entities.TaskItem task, Domain.Entities.User user) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status.ToString(),
        AssignedUserId = task.AssignedUserId,
        AssignedUserName = user.Name,
        AssignedUserEmail = user.Email,
        CreatedAt = task.CreatedAt,
        AdditionalInfo = task.AdditionalInfo
    };
}
