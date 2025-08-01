using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace FinanceService.Infrastructure;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Password).HasColumnName("password").IsRequired();
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("currency");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Rate).HasColumnName("rate").HasColumnType("decimal(18,6)");
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.ToTable("user_favourites");
            entity.HasKey(e => new { e.UserId, e.CurrencyId });
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CurrencyId).HasColumnName("currency_id");

            entity.HasOne(uf => uf.User)
                .WithMany()
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uf => uf.Currency)
                .WithMany(c => c.UserFavorites)
                .HasForeignKey(uf => uf.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 