namespace TaskManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task AddAsync(Entities.User user);
    Task<IEnumerable<Entities.User>> GetAllAsync();
    Task<bool> ExistsByEmailAsync(string email);
    Task<Entities.User?> GetByIdAsync(Guid id);
}
