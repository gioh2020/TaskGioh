namespace TaskManagement.Application.DTOs.Users;

public class UploadPhotoResponseDto
{
    public Guid UserId { get; set; }
    public string PhotoPath { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
