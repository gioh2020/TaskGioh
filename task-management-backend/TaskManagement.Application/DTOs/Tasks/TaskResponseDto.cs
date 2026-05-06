namespace TaskManagement.Application.DTOs.Tasks;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid AssignedUserId { get; set; }
    public string AssignedUserName { get; set; } = string.Empty;
    public string AssignedUserEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? AdditionalInfo { get; set; }
}
