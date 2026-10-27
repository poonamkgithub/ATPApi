using ATPApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ATPApi.Data;

public class AtpDbContext : DbContext
{
    public AtpDbContext(DbContextOptions<AtpDbContext> options) : base(options) { }

    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<IncomingSupply> IncomingSupplies => Set<IncomingSupply>();
    public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>().HasIndex(x => new { x.ProductCode, x.LocationCode }).IsUnique();
        modelBuilder.Entity<IncomingSupply>().HasIndex(x => new { x.ProductCode, x.LocationCode, x.ExpectedDate });
        modelBuilder.Entity<CustomerOrder>().HasIndex(x => new { x.ProductCode, x.LocationCode, x.RequiredDate });
        base.OnModelCreating(modelBuilder);
    }
}
