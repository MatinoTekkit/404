using SportReservation.Data;

namespace SportReservation.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            await context.Response.WriteAsync(exception.Message);
        }
    }
}