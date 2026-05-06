using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;
using DomainTask = TaskManagement.Domain.Entities.TaskItem;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DomainTask task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task<IEnumerable<DomainTask>> GetAllAsync()
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DomainTask>> GetByStatusAsync(TaskStatus status)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<DomainTask?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<DomainTask>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .Where(t => t.AssignedUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DomainTask>> GetByJsonPriorityAsync(string priority)
    {
        var priorityParam = new SqlParameter("@priority", priority.Trim().ToLowerInvariant());

        return await _context.Tasks
            .FromSqlRaw(
                @"SELECT Id, Title, Description, Status, AssignedUserId, CreatedAt, AdditionalInfo
                  FROM Tasks
                  WHERE AdditionalInfo IS NOT NULL
                    AND ISJSON(AdditionalInfo) = 1
                    AND JSON_VALUE(AdditionalInfo, '$.priority') = @priority
                  ORDER BY CreatedAt DESC",
                priorityParam)
            .AsNoTracking()
            .Include(t => t.AssignedUser)
            .ToListAsync();
    }
}
