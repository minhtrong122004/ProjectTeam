using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class VehicleModel
{
    public int ModelId { get; set; }

    public string Name { get; set; } = null!;

    public string BatteryName { get; set; } = null!;

    public virtual ICollection<Battery> Batteries { get; set; } = new List<Battery>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
