using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class SwapTransaction
{
    public int SwapId { get; set; }

    public int? ReservationId { get; set; }

    public int UserId { get; set; }

    public int VehicleId { get; set; }

    public int StationId { get; set; }

    public int FromBatteryId { get; set; }

    public int ToBatteryId { get; set; }

    public DateTime? SwapTime { get; set; }

    public int StaffId { get; set; }

    public string? Status { get; set; }

    public virtual Battery FromBattery { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Reservation? Reservation { get; set; }

    public virtual User Staff { get; set; } = null!;

    public virtual Station Station { get; set; } = null!;

    public virtual Battery ToBattery { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
