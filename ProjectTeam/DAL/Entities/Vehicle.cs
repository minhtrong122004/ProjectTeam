using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public string Vin { get; set; } = null!;

    public int ModelId { get; set; }

    public int? CurrentBatteryId { get; set; }

    public virtual Battery? CurrentBattery { get; set; }

    public virtual VehicleModel Model { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<SwapTransaction> SwapTransactions { get; set; } = new List<SwapTransaction>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
