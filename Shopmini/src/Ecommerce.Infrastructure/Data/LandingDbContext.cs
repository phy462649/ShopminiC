using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Infrastructure.Data;

public partial class LandingDbContext : DbContext
{
    public LandingDbContext()
    {
    }

    public LandingDbContext(DbContextOptions<LandingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<booking> bookings { get; set; }

    public virtual DbSet<booking_service> booking_services { get; set; }

    public virtual DbSet<customer> customers { get; set; }

    public virtual DbSet<order> orders { get; set; }

    public virtual DbSet<order_item> order_items { get; set; }

    public virtual DbSet<payment> payments { get; set; }

    public virtual DbSet<product> products { get; set; }

    public virtual DbSet<role> roles { get; set; }

    public virtual DbSet<room> rooms { get; set; }

    public virtual DbSet<service> services { get; set; }

    public virtual DbSet<staff> staff { get; set; }

    public virtual DbSet<staff_schedule> staff_schedules { get; set; }

    public virtual DbSet<view_top_selling_product> view_top_selling_products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=servicemassage;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.0.1-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<booking>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("booking");

            entity.HasIndex(e => e.customer_id, "customer_id");

            entity.HasIndex(e => new { e.room_id, e.start_time }, "idx_booking_room_time");

            entity.HasIndex(e => new { e.staff_id, e.start_time }, "idx_booking_staff_time");

            entity.HasIndex(e => e.status, "idx_booking_status");

            entity.HasIndex(e => e.start_time, "idx_booking_time");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.end_time).HasColumnType("datetime");
            entity.Property(e => e.start_time).HasColumnType("datetime");
            entity.Property(e => e.status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','completed','cancelled')");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.customer).WithMany(p => p.bookings)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_ibfk_1");

            entity.HasOne(d => d.room).WithMany(p => p.bookings)
                .HasForeignKey(d => d.room_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_ibfk_3");

            entity.HasOne(d => d.staff).WithMany(p => p.bookings)
                .HasForeignKey(d => d.staff_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_ibfk_2");
        });

        modelBuilder.Entity<booking_service>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("booking_service");

            entity.HasIndex(e => e.booking_id, "booking_id");

            entity.HasIndex(e => e.service_id, "service_id");

            entity.Property(e => e.price).HasPrecision(10, 2);
            entity.Property(e => e.quantity).HasDefaultValueSql("'1'");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.booking).WithMany(p => p.booking_services)
                .HasForeignKey(d => d.booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_service_ibfk_1");

            entity.HasOne(d => d.service).WithMany(p => p.booking_services)
                .HasForeignKey(d => d.service_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_service_ibfk_2");
        });

        modelBuilder.Entity<customer>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.Property(e => e.address).HasMaxLength(255);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<order>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => new { e.customer_id, e.order_time }, "idx_orders_customer_time");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.order_time).HasColumnType("datetime");
            entity.Property(e => e.status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','shipped','completed','cancelled')");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.customer).WithMany(p => p.orders)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<order_item>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.product_id, "idx_order_items_product");

            entity.HasIndex(e => e.order_id, "order_id");

            entity.Property(e => e.price).HasPrecision(10, 2);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.order).WithMany(p => p.order_items)
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_ibfk_1");

            entity.HasOne(d => d.product).WithMany(p => p.order_items)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_ibfk_2");
        });

        modelBuilder.Entity<payment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("payment");

            entity.HasIndex(e => new { e.payment_type, e.booking_id, e.order_id }, "idx_payment_type_ref");

            entity.HasIndex(e => e.booking_id, "payment_fk_booking");

            entity.HasIndex(e => e.order_id, "payment_fk_order");

            entity.Property(e => e.amount).HasPrecision(10, 2);
            entity.Property(e => e.created_by).HasMaxLength(100);
            entity.Property(e => e.method)
                .HasDefaultValueSql("'cash'")
                .HasColumnType("enum('cash','card','bank_transfer','momo','zalopay')");
            entity.Property(e => e.payment_time)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.payment_type).HasColumnType("enum('booking','order')");
            entity.Property(e => e.status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','completed','failed')");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.booking).WithMany(p => p.payments)
                .HasForeignKey(d => d.booking_id)
                .HasConstraintName("payment_fk_booking");

            entity.HasOne(d => d.order).WithMany(p => p.payments)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("payment_fk_order");
        });

        modelBuilder.Entity<product>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.name, "idx_product_name");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.price).HasPrecision(10, 2);
            entity.Property(e => e.stock).HasDefaultValueSql("'0'");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("role");

            entity.HasIndex(e => e.name, "name").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<room>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("room");

            entity.Property(e => e.capacity).HasDefaultValueSql("'1'");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<service>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("service");

            entity.HasIndex(e => e.name, "idx_service_name");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.price).HasPrecision(10, 2);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<staff>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.HasIndex(e => e.specialty, "idx_staff_specialty");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.specialty).HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasMany(d => d.roles).WithMany(p => p.staff)
                .UsingEntity<Dictionary<string, object>>(
                    "staff_role",
                    r => r.HasOne<role>().WithMany()
                        .HasForeignKey("role_id")
                        .HasConstraintName("staff_role_ibfk_2"),
                    l => l.HasOne<staff>().WithMany()
                        .HasForeignKey("staff_id")
                        .HasConstraintName("staff_role_ibfk_1"),
                    j =>
                    {
                        j.HasKey("staff_id", "role_id")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("staff_role");
                        j.HasIndex(new[] { "role_id" }, "role_id");
                    });
        });

        modelBuilder.Entity<staff_schedule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("staff_schedule");

            entity.HasIndex(e => e.staff_id, "idx_staff_schedule_staff");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.day_of_week).HasComment("0=Sun..6=Sat");
            entity.Property(e => e.end_time).HasColumnType("time");
            entity.Property(e => e.is_working).HasDefaultValueSql("'1'");
            entity.Property(e => e.start_time).HasColumnType("time");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.staff).WithMany(p => p.staff_schedules)
                .HasForeignKey(d => d.staff_id)
                .HasConstraintName("fk_staff_schedule_staff");
        });

        modelBuilder.Entity<view_top_selling_product>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_top_selling_products");

            entity.Property(e => e.product_name).HasMaxLength(100);
            entity.Property(e => e.revenue).HasPrecision(42, 2);
            entity.Property(e => e.total_sold).HasPrecision(32);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
