using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<UserResponseDto>.Failure("El nombre es obligatorio.", 400);

        if (string.IsNullOrWhiteSpace(dto.Email))
            return Result<UserResponseDto>.Failure("El correo electrónico es obligatorio.", 400);

        if (!dto.Email.Contains('@'))
            return Result<UserResponseDto>.Failure("El formato del correo electrónico no es válido.", 400);

        var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(dto.Email);
        if (emailExists)
            return Result<UserResponseDto>.Failure($"Ya existe un usuario con el correo '{dto.Email}'.", 409);

        var user = User.Create(dto.Name, dto.Email);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<UserResponseDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return Result<UserResponseDto>.Failure("Usuario no encontrado.", 404);

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<UserResponseDto>.Failure("El nombre es obligatorio.", 400);

        user.UpdateName(dto.Name);
        await _unitOfWork.SaveChangesAsync();

        return Result<UserResponseDto>.Success(MapToDto(user));
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return Result<IEnumerable<UserResponseDto>>.Success(users.Select(MapToDto));
    }

    private UserResponseDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    };
}
