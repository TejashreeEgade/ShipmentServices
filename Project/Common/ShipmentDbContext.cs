using Microsoft.EntityFrameworkCore;
using Project.Model;
using System.Reflection.Emit;

namespace Project.Common
{
    public class ShipmentDbContext : DbContext
    {
        public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShipmentModel> Shipments => Set<ShipmentModel>();

        public DbSet<ShipmentHistoryModel> ShipmentStatusHistories => Set<ShipmentHistoryModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShipmentModel>()
                .HasIndex(x => x.shipmentNumber)
                .IsUnique();

            modelBuilder.Entity<ShipmentModel>()
                .HasMany(x => x.statusHistories)
                .WithOne(x => x.shipment)
                .HasForeignKey(x => x.shipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
