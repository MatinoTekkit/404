using Microsoft.EntityFrameworkCore;
using SportReservation.Models;
using SportReservation.Data;

namespace SportReservation.Services;

public class ReservationService
{
    private readonly AppDbContext _db;

    public ReservationService(AppDbContext db)
    {
        _db = db;
    }

    // Vrátí rezervaci podle id (včetně navigačních vlastností)
    public async Task<Reservation?> GetReservationAsync(Guid id)
    {
        return await _db.Reservations
            .Include(r => r.User)
            .Include(r => r.Facility)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Reservation> CreateReservationAsync(Guid userId, Guid facilityId, DateTime startAt,
        DateTime endAt)
    {
        // 1. Kontrola kolize – překrývající se aktivní rezervace
        bool collision = await _db.Reservations.AnyAsync(r =>
            r.FacilityId == facilityId &&
            r.Status == ReservationStatus.Active &&
            r.StartAt < endAt &&
            r.EndAt > startAt);

        if (collision)
            throw new Exception("Sportoviště je v tomto čase již rezervováno.");

        // 2. Kontrola odstávky
        bool downtime = await _db.Downtimes.AnyAsync(d =>
            d.FacilityId == facilityId &&
            d.StartAt < endAt &&
            d.EndAt > startAt);

        if (downtime)
            throw new Exception("Sportoviště je v tomto čase mimo provoz.");

        var facility = await _db.Facilities.FirstAsync(f => f.Id == facilityId);

        // 3. Aktuální ceník
        var priceList = await _db.PriceLists
            .Where(p => p.FacilityTypeId == facility.TypeId &&
                        p.ValidFrom <= startAt &&
                        (p.ValidTo == null || p.ValidTo >= endAt))
            .FirstOrDefaultAsync();

        if (priceList == null)
            throw new Exception("Pro sportoviště není nastaven ceník.");

        // 4. Sleva podle počtu předchozích rezervací (5/10/15 → 5/10/15%)
        int reservationCount = await _db.Reservations.CountAsync(r =>
            r.UserId == userId &&
            r.Status == ReservationStatus.Active);

        int discount = 0;
        if (reservationCount >= 15) discount = 15;
        else if (reservationCount >= 10) discount = 10;
        else if (reservationCount >= 5) discount = 5;

        // 5. Výpočet ceny
        double hours = (endAt - startAt).TotalHours;
        decimal basePrice = Math.Round((decimal)hours * priceList.PricePerHour, 2);
        decimal finalPrice = Math.Round(basePrice * (1 - discount / 100m), 2);

        // 6. Uložení
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FacilityId = facilityId,
            StartAt = startAt,
            EndAt = endAt,
            Status = ReservationStatus.Active,
            BasePrice = basePrice,
            DiscountPercent = discount,
            FinalPrice = finalPrice,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
        return reservation;
    }

    // Jednoduchá implementace zrušení rezervace
    public async Task CancelReservationAsync(Guid reservationId, Guid userId, bool isAdmin)
    {
        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.Id == reservationId);
        if (r == null) throw new InvalidOperationException("Reservation not found.");

        if (!isAdmin && r.UserId != userId)
            throw new UnauthorizedAccessException("User is not allowed to cancel this reservation.");

        r.CancelledAt = DateTime.UtcNow;
        // pokud existuje enum s hodnotou Cancelled, použít ji
        try
        {
            r.Status = ReservationStatus.Cancelled;
        }
        catch
        {
            // pokud enum neobsahuje Cancelled, ignorujeme přiřazení
        }

        _db.Reservations.Update(r);
        await _db.SaveChangesAsync();
    }
}