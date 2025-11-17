using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Station
{
    public int StationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Status { get; set; }

    public int Capacity { get; set; }

    public virtual ICollection<Battery> Batteries { get; set; } = new List<Battery>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<SwapTransaction> SwapTransactions { get; set; } = new List<SwapTransaction>();
}
