using CoffeeShop.Domain;
using CounterService.Domain;
using Microsoft.EntityFrameworkCore;
using N8T.Infrastructure.EfCore;

namespace CoffeeShop.Infrastructure.Data;

public class MainDbContext : AppDbContextBase
{
    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<LineItem> LineItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // Orders
        modelBuilder.Entity<Order>().ToTable("orders", "order");
        modelBuilder.Entity<Order>().HasKey(x => x.Id);
        modelBuilder.Entity<Order>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Order>().Property(x => x.LoyaltyMemberId).HasColumnType("uuid");
        modelBuilder.Entity<Order>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Order>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Order>().Ignore(x => x.DomainEvents);

        modelBuilder.Entity<Order>().Property(x => x.OrderSource).IsRequired();
        modelBuilder.Entity<Order>().Property(x => x.OrderStatus).IsRequired();
        modelBuilder.Entity<Order>().Property(x => x.Location).IsRequired();

        modelBuilder.Entity<LineItem>().ToTable("line_items", "order");
        modelBuilder.Entity<LineItem>().HasKey(x => x.Id);
        modelBuilder.Entity<LineItem>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<LineItem>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);
        modelBuilder.Entity<LineItem>().HasIndex(x => x.Id).IsUnique();

        // relationships
        modelBuilder.Entity<Order>()
            .HasMany(x => x.LineItems);
    }
}
