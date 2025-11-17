using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? SwapId { get; set; }

    public int? ReservationId { get; set; }

    public int? SubscriptionId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string? Method { get; set; }

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Reservation? Reservation { get; set; }

    public virtual UserSubscription? Subscription { get; set; }

    public virtual SwapTransaction? Swap { get; set; }

    public virtual User User { get; set; } = null!;
}
