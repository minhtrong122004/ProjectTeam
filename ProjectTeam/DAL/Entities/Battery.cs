using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Battery
{
    public int BatteryId { get; set; }

    public int StationId { get; set; }

    public int ModelId { get; set; }

    public int CapacityKw { get; set; }

    public string Status { get; set; } = null!;

    public double Soh { get; set; }

    public virtual VehicleModel Model { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Station Station { get; set; } = null!;

    public virtual ICollection<SwapTransaction> SwapTransactionFromBatteries { get; set; } = new List<SwapTransaction>();

    public virtual ICollection<SwapTransaction> SwapTransactionToBatteries { get; set; } = new List<SwapTransaction>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
