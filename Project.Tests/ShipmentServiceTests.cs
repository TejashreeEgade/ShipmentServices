using Microsoft.EntityFrameworkCore;
using Project.Common;
using Project.Model;
using Project.Repository.Implementation;
using Project.Service.IService;
using Project.Service.Implementation;
using Project.Enum;
using System.Threading.Tasks;
using Xunit;

namespace Project.Tests
{
    public class ShipmentServiceTests
    {
        private ShipmentDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ShipmentDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ShipmentDbContext(options);
        }

        [Fact]
        public async Task AddShipment_ValidShipment_SetsDraftAndSaves()
        {
            using var context = CreateInMemoryContext("AddShipment_Valid");

            var repository = new ShipmentModelRepository(context);
            var service = new ShipmentService(repository);

            var shipment = new ShipmentModel
            {
                shipmentNumber = "SHP-001",
                customerName = "Cust",
                origin = "A",
                destination = "B",
                mode = ShipmentMode.Air,
                estimatedRevenue = 100m,
                estimatedCost = 50m,
                expectedDepartureDate = System.DateTime.UtcNow.AddDays(1),
                expectedArrivalDate = System.DateTime.UtcNow.AddDays(2),
                branchId = 1,
                assignedEmployeeId = 1
            };

            await service.AddShipment(shipment);

            var saved = await context.Shipments.FirstOrDefaultAsync(x => x.shipmentNumber == "SHP-001");

            Assert.NotNull(saved);
            Assert.Equal(ShipmentStatus.Draft, saved.status);
            Assert.True(saved.createdAt != default);
        }

        [Fact]
        public async Task AddShipment_NegativeRevenue_ThrowsException()
        {
            using var context = CreateInMemoryContext("AddShipment_NegativeRevenue");

            var repository = new ShipmentModelRepository(context);
            var service = new ShipmentService(repository);

            var shipment = new ShipmentModel
            {
                shipmentNumber = "SHP-002",
                customerName = "Cust",
                origin = "A",
                destination = "B",
                mode = ShipmentMode.Sea,
                estimatedRevenue = -1m,
                estimatedCost = 50m,
                expectedDepartureDate = System.DateTime.UtcNow.AddDays(1),
                expectedArrivalDate = System.DateTime.UtcNow.AddDays(2),
                branchId = 1,
                assignedEmployeeId = 1
            };

            await Assert.ThrowsAsync<System.Exception>(() => service.AddShipment(shipment));
        }

        [Fact]
        public async Task GetShipmentById_NotFound_ThrowsException()
        {
            using var context = CreateInMemoryContext("GetById_NotFound");

            var repository = new ShipmentModelRepository(context);
            var service = new ShipmentService(repository);

            await Assert.ThrowsAsync<System.Exception>(() => service.GetShipmentById(999));
        }

        [Fact]
        public async Task Update_ChangesStatus_AddsHistory()
        {
            using var context = CreateInMemoryContext("Update_AddsHistory");

            var shipment = new ShipmentModel
            {
                shipmentNumber = "SHP-003",
                customerName = "Cust",
                origin = "A",
                destination = "B",
                mode = ShipmentMode.Land,
                estimatedRevenue = 200m,
                estimatedCost = 100m,
                expectedDepartureDate = System.DateTime.UtcNow.AddDays(-2),
                expectedArrivalDate = System.DateTime.UtcNow.AddDays(1),
                branchId = 1,
                assignedEmployeeId = 1,
                status = ShipmentStatus.Draft,
                createdAt = System.DateTime.UtcNow
            };

            context.Shipments.Add(shipment);
            await context.SaveChangesAsync();

            var repository = new ShipmentModelRepository(context);
            var service = new ShipmentService(repository);

            await service.Update(shipment.Id, ShipmentStatus.Delivered, changedBy: 10, remarks: "Done");

            var updated = await context.Shipments.Include(x => x.statusHistories).FirstAsync(x => x.Id == shipment.Id);

            Assert.Equal(ShipmentStatus.Delivered, updated.status);
            Assert.Single(updated.statusHistories);
            Assert.Equal(ShipmentStatus.Draft, updated.statusHistories.First().previosStatus);
            Assert.Equal(ShipmentStatus.Delivered, updated.statusHistories.First().newStatus);
        }

        [Fact]
        public async Task Delete_RemovesShipment()
        {
            using var context = CreateInMemoryContext("Delete_Removes");

            var shipment = new ShipmentModel
            {
                shipmentNumber = "SHP-004",
                customerName = "Cust",
                origin = "A",
                destination = "B",
                mode = ShipmentMode.Land,
                estimatedRevenue = 50m,
                estimatedCost = 25m,
                expectedDepartureDate = System.DateTime.UtcNow,
                expectedArrivalDate = System.DateTime.UtcNow.AddDays(1),
                branchId = 1,
                assignedEmployeeId = 1,
                status = ShipmentStatus.Draft,
                createdAt = System.DateTime.UtcNow
            };

            context.Shipments.Add(shipment);
            await context.SaveChangesAsync();

            var repository = new ShipmentModelRepository(context);
            var service = new ShipmentService(repository);

            await service.Delete(shipment.Id);

            var count = await context.Shipments.CountAsync();
            Assert.Equal(0, count);
        }
    }
}
