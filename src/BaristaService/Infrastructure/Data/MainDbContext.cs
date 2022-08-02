using BaristaService.Domain;
using Microsoft.EntityFrameworkCore;
using N8T.Infrastructure.EfCore;

namespace CoffeeShop.Infrastructure.Data;

public class MainDbContext : AppDbContextBase
{
    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<BaristaItem> BaristaItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // BaristaItems
        modelBuilder.Entity<BaristaItem>().ToTable("barista_orders", "barista");
        modelBuilder.Entity<BaristaItem>().HasKey(x => x.Id);
        modelBuilder.Entity<BaristaItem>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<BaristaItem>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<BaristaItem>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<BaristaItem>().Ignore(x => x.DomainEvents);

        modelBuilder.Entity<BaristaItem>().Property(x => x.ItemType).IsRequired();
        modelBuilder.Entity<BaristaItem>().Property(x => x.ItemName).IsRequired();
    }
}
