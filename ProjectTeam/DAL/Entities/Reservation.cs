using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public int VehicleId { get; set; }

    public int StationId { get; set; }

    public int? BatteryId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }

    public virtual Battery? Battery { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Station Station { get; set; } = null!;

    public virtual ICollection<SwapTransaction> SwapTransactions { get; set; } = new List<SwapTransaction>();

    public virtual User User { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
