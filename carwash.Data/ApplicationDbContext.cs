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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.QrCode).HasMaxLength(100);
            entity.HasIndex(u => u.QrCode)
                .IsUnique()
                .HasFilter("[QrCode] IS NOT NULL");
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
        });
    }
}
