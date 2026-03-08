namespace SportReservation.Models;

public enum UserRole
{
    User = 0,
    Admin = 1
}

public enum ReservationStatus
{
    Active = 0,
    Cancelled = 1
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public string FullName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public class FacilityType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
}

public class Facility
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TypeId { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual FacilityType Type { get; set; } = null!;
    public virtual ICollection<PriceList> PriceLists { get; set; } = new List<PriceList>();
    public virtual ICollection<Downtime> Downtimes { get; set; } = new List<Downtime>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public class Reservation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public ReservationStatus Status { get; set; }
    public decimal BasePrice { get; set; }
    public int DiscountPercent { get; set; }
    public decimal FinalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Facility Facility { get; set; } = null!;
}

public class PriceList
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public decimal PricePerHour { get; set; }

    public virtual Facility Facility { get; set; } = null!;
}

public class Downtime
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Reason { get; set; } = null!;

    public virtual Facility Facility { get; set; } = null!;
}