using Microsoft.EntityFrameworkCore;
using SportReservation.Models;

namespace SportReservation.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<FacilityType> FacilityTypes { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<PriceList> PriceLists { get; set; } = null!;
    public DbSet<Downtime> Downtimes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User – email musí být unikátní
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Enumy uložit jako text (čitelnější v databázi)
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Reservation>()
            .Property(r => r.Status)
            .HasConversion<string>();

        // Decimal –  ceny (10 celých, 2 desetinné) 
        modelBuilder.Entity<Reservation>()
            .Property(r => r.BasePrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Reservation>()
            .Property(r => r.FinalPrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<PriceList>()
            .Property(p => p.PricePerHour)
            .HasColumnType("decimal(10,2)");

        // Facility -> FacilityType: při smazání typu zakázat (aby nešly smazat typy s existujícími sportovišti)
        modelBuilder.Entity<Facility>()
            .HasOne(f => f.Type)
            .WithMany(t => t.Facilities)
            .HasForeignKey(f => f.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Explicitní relace pro srozumitelnost a konzistentní chování při smazání
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Facility)
            .WithMany(f => f.Reservations)
            .HasForeignKey(r => r.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PriceList>()
            .HasOne(p => p.Facility)
            .WithMany(f => f.PriceLists)
            .HasForeignKey(p => p.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Downtime>()
            .HasOne(d => d.Facility)
            .WithMany(f => f.Downtimes)
            .HasForeignKey(d => d.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}