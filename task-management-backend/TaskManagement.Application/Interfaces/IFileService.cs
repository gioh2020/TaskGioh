namespace TaskManagement.Application.Interfaces;

public interface IFileService
{
    Task<string> SaveImageAsync(Stream imageStream, string originalFileName);
    void DeleteImage(string filePath);
    bool IsValidImageExtension(string fileName);
}
