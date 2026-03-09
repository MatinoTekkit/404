using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SportReservation.Data;
using SportReservation.Models;

namespace SportReservation.Services;

public class UserService(AppDbContext db)
{
    private const string EmailPattern = "[a-zA-Z\\d._-]+@[a-zA-Z\\d.-]+\\.[a-zA-Z]{2,4}";

    public async Task<User> Register(RegisterDto dto, UserRole role)
    {
        if (!Regex.IsMatch(dto.Email, EmailPattern))
        {
            throw new BadHttpRequestException("email-invalid");
        }

        if (await db.Users.AnyAsync(it => it.Email == dto.Email))
        {
            throw new BadHttpRequestException("already-exists");
        }

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            CreatedAt = DateTime.Now,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
        };

        await db.AddAsync(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task Update(User user, UserPatchDto patch)
    {
        if (patch.Email != null)
        {
            user.Email = patch.Email;
        }

        if (patch.FullName != null)
        {
            user.FullName = patch.FullName;
        }

        db.Update(user);
        await db.SaveChangesAsync();
    }
}