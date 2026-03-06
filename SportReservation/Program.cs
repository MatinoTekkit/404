using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SportReservation.Data;
using SportReservation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=sportreservation.db")
);

builder.Services.AddScoped<ReservationService>();

builder.Services.AddControllers()
    .AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // adresa FE
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// SPA frontend in wwwroot
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

//  [Authorize]
app.UseAuthorization();

app.MapControllers();
app.UseRouting();

app.Run();