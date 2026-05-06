using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Users;

namespace TaskManagement.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto);
    Task<Result<UserResponseDto>> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<Result<IEnumerable<UserResponseDto>>> GetAllAsync();
}
