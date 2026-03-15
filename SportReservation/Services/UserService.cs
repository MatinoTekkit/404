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

    public async Task<User> Update(User invoker, UserPatchDto patch)
    {
        var user = await GetUser(invoker, patch);

        if (patch.Email != null)
        {
            if (await db.Users.AnyAsync(it => it != user && it.Email == user.Email))
            {
                throw new BadHttpRequestException("email-already-exists");
            }

            user.Email = patch.Email;
        }

        if (patch.FullName != null)
        {
            user.FullName = patch.FullName;
        }

        if (patch.Password != null)
        {
            if (invoker.Role == UserRole.Admin)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(patch.Password.New);
            }
            else
            {
                if (patch.Password.Current == null)
                {
                    throw new BadHttpRequestException("password-required");
                }

                if (!BCrypt.Net.BCrypt.Verify(patch.Password.Current, user.PasswordHash))
                {
                    throw new BadHttpRequestException("wrong-password");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(patch.Password.New);
            }
        }

        db.Update(user);
        await db.SaveChangesAsync();
        return user;
    }

    private async Task<User> GetUser(User invoker, UserPatchDto patch)
    {
        if (patch.Id == null || patch.Id == invoker.Id)
        {
            return invoker;
        }

        if (invoker.Role != UserRole.Admin)
        {
            throw new BadHttpRequestException("forbidden", StatusCodes.Status403Forbidden);
        }

        var user = await db.Users.FirstOrDefaultAsync(it => it.Id == patch.Id);
        return user ?? throw new BadHttpRequestException("not-found", StatusCodes.Status404NotFound);
    }
}