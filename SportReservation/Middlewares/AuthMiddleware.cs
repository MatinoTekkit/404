using System.Text;
using Microsoft.EntityFrameworkCore;
using SportReservation.Data;
using SportReservation.Models;

namespace SportReservation.Middlewares;

public class AuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var path = context.Request.Path;

        if (!path.StartsWithSegments("/api") || path.StartsWithSegments("/api/Public"))
        {
            await next(context);
            return;
        }

        var isAuthenticated = false;

        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var headerValue = authHeader.ToString();

            if (headerValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var encodedCredentials = headerValue.Substring(6).Trim();
                    var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                    var parts = decodedString.Split(':', 2);

                    if (parts.Length == 2)
                    {
                        var email = parts[0];
                        var password = parts[1];

                        var user = await db.Users
                            .Where(u => u.Email == email)
                            .FirstOrDefaultAsync();

                        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                        {
                            isAuthenticated = true;
                            context.Items.Add("User", user);
                        }
                    }
                }
                catch
                {
                    // hodne dotnet syntax btw
                }
            }
        }

        if (!isAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid credentials" });
            return;
        }

        await next(context);
    }
}

public static class AuthMiddlewareExtensions
{
    public static User LoggedUser(this HttpContext context)
    {
        return context.Items["User"] as User ?? throw new Exception("Not logged?");
    }
}