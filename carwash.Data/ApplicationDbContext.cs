using carwash.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace carwash.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserCar> UserCars => Set<UserCar>();
    public DbSet<WashService> WashServices => Set<WashService>();
    public DbSet<WashRecord> WashRecords => Set<WashRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.QrCode).HasMaxLength(100);
            entity.HasIndex(u => u.CreatedAt);
            entity.HasIndex(u => u.QrCode)
                .IsUnique()
                .HasFilter("[QrCode] IS NOT NULL");
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
        });

        builder.Entity<UserCar>(entity =>
        {
            entity.Property(c => c.CarType).HasMaxLength(100).IsRequired();
            entity.Property(c => c.PlateNumber).HasMaxLength(20).IsRequired();
            entity.Property(c => c.Size).HasConversion<string>().HasMaxLength(10);

            entity.HasOne(c => c.User)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.UserId, c.PlateNumber }).IsUnique();
        });

        builder.Entity<WashService>(entity =>
        {
            entity.Property(s => s.NameAr).HasMaxLength(100).IsRequired();
            entity.Property(s => s.NameEn).HasMaxLength(100).IsRequired();
            entity.HasIndex(s => s.NameAr).IsUnique();
            entity.HasIndex(s => s.NameEn).IsUnique();

            entity.HasData(
                new WashService { Id = 1, NameAr = "غسلة عادية", NameEn = "Normal Wash", Points = 30 },
                new WashService { Id = 2, NameAr = "غسلة مميزة", NameEn = "Premium Wash", Points = 50 },
                new WashService { Id = 3, NameAr = "غسلة مجانية", NameEn = "Free Wash", Points = -250 });
        });

        builder.Entity<WashRecord>(entity =>
        {
            entity.Property(r => r.UserFullName).HasMaxLength(100).IsRequired();
            entity.Property(r => r.PlateNumber).HasMaxLength(20).IsRequired();
            entity.Property(r => r.CarType).HasMaxLength(100).IsRequired();
            entity.Property(r => r.CarSize).HasConversion<string>().HasMaxLength(10);
            entity.Property(r => r.ServiceNameAr).HasMaxLength(100).IsRequired();
            entity.Property(r => r.ServiceNameEn).HasMaxLength(100).IsRequired();
            entity.Property(r => r.AmountPaid).HasColumnType("decimal(18,2)");
            entity.Property(r => r.PaymentMethod).HasConversion<string>().HasMaxLength(10);

            entity.HasIndex(r => r.CreatedAt);
            entity.HasIndex(r => new { r.UserId, r.CreatedAt });
            entity.HasIndex(r => new { r.CarId, r.CreatedAt });
        });
    }
}
