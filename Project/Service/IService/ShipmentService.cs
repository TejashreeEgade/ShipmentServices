using Microsoft.EntityFrameworkCore;
using Project.Enum;
using Project.Model;
using Project.Repository.IRepository;
using Project.Service.Implementation;

namespace Project.Service.IService
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _repository;

        public ShipmentService(IShipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task AddShipment(ShipmentModel shipmentModel)
        {
            // Validation

            if (shipmentModel.estimatedRevenue < 0)
                throw new Exception("Estimated Revenue cannot be negative.");

            if (shipmentModel.estimatedCost < 0)
                throw new Exception("Estimated Cost cannot be negative.");

            if (shipmentModel.expectedArrivalDate <= shipmentModel.expectedDepartureDate)
                throw new Exception("Arrival Date should be greater than Departure Date.");

            shipmentModel.status = ShipmentStatus.Draft;

            shipmentModel.createdAt = DateTime.Now;

            await _repository.AddShipment(shipmentModel);

            await _repository.SaveChangesAsync();
        }

        // Get Shipment By Id
        public async Task<ShipmentModel> GetShipmentById(int id)
        {
            var shipment = await _repository.GetShipmentById(id);

            if (shipment == null)
                throw new Exception("Shipment not found.");

            return shipment;
        }

        // Get All Shipments
        public async Task<List<ShipmentModel>> GetAll()
        {
            return await _repository.GetAll()
                                    .OrderByDescending(x => x.createdAt)
                                    .ToListAsync();
        }

        // Update Shipment Status
        public async Task Update(int shipmentId,
                                               ShipmentStatus newStatus,
                                               int changedBy,
                                               string? remarks)
        {
            var shipment = await _repository.GetShipmentById(shipmentId);

            if (shipment == null)
                throw new Exception("Shipment not found.");

            // Business Rule

            if (shipment.status == ShipmentStatus.Closed ||
                shipment.status == ShipmentStatus.Cancelled)
            {
                throw new Exception("Closed/Cancelled shipment cannot be updated.");
            }

            ShipmentHistoryModel history = new ShipmentHistoryModel
            {
                shipmentId = shipment.Id,
                previosStatus = shipment.status,
                newStatus = newStatus,
                changedBy = changedBy,
                remarks = remarks,
                changedAt = DateTime.UtcNow
            };

            await _repository.AddStatusHistory(history);

            shipment.status = newStatus;

            shipment.updatedAt = DateTime.UtcNow;

            _repository.Update(shipment);

            await _repository.SaveChangesAsync();
        }

        // Delete Shipment
        public async Task Delete(int id)
        {
            var shipment = await _repository.GetShipmentById(id);

            if (shipment == null)
                throw new Exception("Shipment not found.");

            _repository.Delete(shipment);

            await _repository.SaveChangesAsync();
        }

        // Dashboard Summary
        public async Task<object> GetSummary()
        {
            var shipments = _repository.GetAll();

            var totalShipment = await shipments.CountAsync();

            var totalRevenue = await shipments.SumAsync(x => x.estimatedRevenue);

            var totalCost = await shipments.SumAsync(x => x.estimatedCost);

            var totalProfit = totalRevenue - totalCost;

            var delayedShipment = await shipments.CountAsync(x =>
                x.expectedArrivalDate < DateTime.UtcNow &&
                x.status != ShipmentStatus.Delivered &&
                x.status != ShipmentStatus.Closed &&
                x.status != ShipmentStatus.Cancelled);

            var shipmentByStatus = await shipments
                .GroupBy(x => x.status)
                .Select(x => new
                {
                    Status = x.Key.ToString(),
                    Count = x.Count()
                })
                .ToListAsync();

            return new
            {
                TotalShipment = totalShipment,
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                TotalProfit = totalProfit,
                DelayedShipment = delayedShipment,
                ShipmentByStatus = shipmentByStatus
            };
        }
    }
}

