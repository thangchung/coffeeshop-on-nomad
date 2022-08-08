using KitchenService.Domain;
using Microsoft.EntityFrameworkCore;
using N8T.Infrastructure.EfCore;

namespace KitchenService.Infrastructure.Data;

public class MainDbContext : AppDbContextBase
{
    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<KitchenOrder> KitchenOrders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // KitchenOrders
        modelBuilder.Entity<KitchenOrder>().ToTable("kitchen_orders", "kitchen");
        modelBuilder.Entity<KitchenOrder>().HasKey(x => x.Id);
        modelBuilder.Entity<KitchenOrder>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<KitchenOrder>().Property(x => x.OrderId).HasColumnType("uuid");
        modelBuilder.Entity<KitchenOrder>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<KitchenOrder>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<KitchenOrder>().Ignore(x => x.DomainEvents);

        modelBuilder.Entity<KitchenOrder>().Property(x => x.ItemType).IsRequired();
        modelBuilder.Entity<KitchenOrder>().Property(x => x.ItemName).IsRequired();
    }
}
