using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace DAL.Entities;

public partial class EvBatterySwapSystemContext : DbContext
{
    public EvBatterySwapSystemContext()
    {
    }

    public EvBatterySwapSystemContext(DbContextOptions<EvBatterySwapSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Battery> Batteries { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SwapTransaction> SwapTransactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleModel> VehicleModels { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Battery>(entity =>
        {
            entity.HasKey(e => e.BatteryId).HasName("PK__BATTERIE__31C8DB8E1AA9972C");

            entity.ToTable("BATTERIES");

            entity.Property(e => e.BatteryId).HasColumnName("battery_id");
            entity.Property(e => e.CapacityKw).HasColumnName("capacity_kw");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Soh).HasColumnName("soh");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Model).WithMany(p => p.Batteries)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BATTERIES_MODEL");

            entity.HasOne(d => d.Station).WithMany(p => p.Batteries)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BATTERIES_STATIONS");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PAYMENTS__ED1FC9EAFDBC3DBC");

            entity.ToTable("PAYMENTS");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("method");
            entity.Property(e => e.PaidAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.SwapId).HasColumnName("swap_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_PAY_RESERV");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK_PAY_SUB");

            entity.HasOne(d => d.Swap).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SwapId)
                .HasConstraintName("FK_PAY_SWAP");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PAY_USER");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__RESERVAT__31384C2996661B52");

            entity.ToTable("RESERVATIONS");

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.BatteryId).HasColumnName("battery_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Battery).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.BatteryId)
                .HasConstraintName("FK_RESERV_BATTERY");

            entity.HasOne(d => d.Station).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESERV_STATION");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESERV_USER");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESERV_VEHICLE");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLES__760965CC4FFAAAD6");

            entity.ToTable("ROLES");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PermissionsJson)
                .HasColumnType("text")
                .HasColumnName("permissions_json");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.StationId).HasName("PK__STATIONS__44B370E98346B696");

            entity.ToTable("STATIONS");

            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__SUBSCRIP__BE9F8F1DAF089955");

            entity.ToTable("SUBSCRIPTION_PLANS");

            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PriorityBooking)
                .HasDefaultValue(false)
                .HasColumnName("priority_booking");
            entity.Property(e => e.SwapLimit).HasColumnName("swap_limit");
        });

        modelBuilder.Entity<SwapTransaction>(entity =>
        {
            entity.HasKey(e => e.SwapId).HasName("PK__SWAP_TRA__2AD51373FE9A7C7D");

            entity.ToTable("SWAP_TRANSACTIONS");

            entity.Property(e => e.SwapId).HasColumnName("swap_id");
            entity.Property(e => e.FromBatteryId).HasColumnName("from_battery_id");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SwapTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("swap_time");
            entity.Property(e => e.ToBatteryId).HasColumnName("to_battery_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.FromBattery).WithMany(p => p.SwapTransactionFromBatteries)
                .HasForeignKey(d => d.FromBatteryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SWAP_FROM_BAT");

            entity.HasOne(d => d.Reservation).WithMany(p => p.SwapTransactions)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_SWAP_RESERV");

            entity.HasOne(d => d.Staff).WithMany(p => p.SwapTransactionStaffs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SWAP_STAFF");

            entity.HasOne(d => d.Station).WithMany(p => p.SwapTransactions)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SWAP_STATION");

            entity.HasOne(d => d.ToBattery).WithMany(p => p.SwapTransactionToBatteries)
                .HasForeignKey(d => d.ToBatteryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SWAP_TO_BAT");

            entity.HasOne(d => d.User).WithMany(p => p.SwapTransactionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SWAP_USER");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.SwapTransactions)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SWAP_VEHICLE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USERS__B9BE370F89B6DCAD");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UQ__USERS__AB6E616458A47A11").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Active")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERS_ROLES");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__USER_SUB__863A7EC1C3C2B1C9");

            entity.ToTable("USER_SUBSCRIPTIONS");

            entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Active")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERSUB_PLAN");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERSUB_USER");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERSUB_VEHICLE");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__VEHICLES__F2947BC1599DB471");

            entity.ToTable("VEHICLES");

            entity.HasIndex(e => e.UserId, "UQ_VEHICLE_USER").IsUnique();

            entity.HasIndex(e => e.Vin, "UQ__VEHICLES__DDB00C66E3FD78EA").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.CurrentBatteryId).HasColumnName("current_battery_id");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Vin)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("vin");

            entity.HasOne(d => d.CurrentBattery).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CurrentBatteryId)
                .HasConstraintName("FK_VEHICLES_BATTERY");

            entity.HasOne(d => d.Model).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VEHICLES_MODEL");

            entity.HasOne(d => d.User).WithOne(p => p.Vehicle)
                .HasForeignKey<Vehicle>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VEHICLES_USERS");
        });

        modelBuilder.Entity<VehicleModel>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PK__VEHICLE___DC39CAF4387C7D25");

            entity.ToTable("VEHICLE_MODELS");

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.BatteryName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("battery_name");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
