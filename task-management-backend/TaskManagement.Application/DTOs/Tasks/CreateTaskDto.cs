namespace TaskManagement.Application.DTOs.Tasks;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AssignedUserId { get; set; }
    public string? AdditionalInfo { get; set; }
}
