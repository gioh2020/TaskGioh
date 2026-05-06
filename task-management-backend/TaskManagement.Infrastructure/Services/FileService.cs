using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class FileService : IFileService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly string _basePath;

    public FileService()
    {
        _basePath = @"C:\TaskManagementImages";
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveImageAsync(Stream imageStream, string originalFileName)
    {
        if (imageStream is null || imageStream.Length == 0)
            throw new ArgumentException("El archivo de imagen está vacío.");

        if (imageStream.Length > MaxFileSizeBytes)
            throw new ArgumentException($"El archivo supera el tamaño máximo permitido de 5 MB.");

        if (!IsValidImageExtension(originalFileName))
            throw new ArgumentException($"Extensión no permitida. Use: {string.Join(", ", AllowedExtensions)}");

        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_basePath, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await imageStream.CopyToAsync(fileStream);

        return fullPath;
    }

    public void DeleteImage(string filePath)
    {
        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            File.Delete(filePath);
    }

    public bool IsValidImageExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return !string.IsNullOrEmpty(extension) && AllowedExtensions.Contains(extension);
    }
}
