using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.DTOs.Tasks;

public class ChangeTaskStatusDto
{
    public TaskStatus NewStatus { get; set; }
}
