namespace TaskManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    private User() { }

    public static User Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El correo electrónico es obligatorio.", nameof(email));

        return new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant()
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(name));

        Name = name.Trim();
    }
}
