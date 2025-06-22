using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Keep unique index on PhoneNumber
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        // Configure UserAddress entity
        builder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Configure foreign key relationship with ApplicationUser
            entity.HasOne(e => e.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure required fields
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Street).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ward).IsRequired().HasMaxLength(100);
            entity.Property(e => e.District).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Province).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            // Configure timestamps
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            // Configure default value for IsDefault
            entity.Property(e => e.IsDefault).HasDefaultValue(false);

            // Create index on UserId for better query performance
            entity.HasIndex(e => e.UserId);

            // Create unique index to ensure only one default address per user
            entity.HasIndex(e => new { e.UserId, e.IsDefault })
                .HasFilter("\"IsDefault\" = true")
                .IsUnique();
        });

        // Configure table names
        builder.Entity<ApplicationUser>().ToTable("users");
        builder.Entity<IdentityRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
        builder.Entity<UserAddress>().ToTable("user_addresses");
    }
}
