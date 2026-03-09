namespace SportReservation.Models;

public record RegisterDto(
    string Email,
    string FullName,
    string Password
);

public record UserDto(
    Guid Id,
    string Email,
    UserRole Role,
    string FullName,
    DateTime CreatedAt
);

public record UserPatchDto(
    Guid Id,
    string? Email,
    string? FullName
);

public static class UserDtoExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            Id: user.Id,
            Email: user.Email,
            Role: user.Role,
            FullName: user.FullName,
            CreatedAt: user.CreatedAt
        );
    }
}