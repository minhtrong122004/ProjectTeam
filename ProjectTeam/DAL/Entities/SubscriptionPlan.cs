using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class SubscriptionPlan
{
    public int PlanId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? SwapLimit { get; set; }

    public bool? PriorityBooking { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
