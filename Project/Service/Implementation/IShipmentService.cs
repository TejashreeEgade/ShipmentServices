using Project.Enum;
using Project.Model;

namespace Project.Service.Implementation
{
    public interface IShipmentService
    {
        Task AddShipment(ShipmentModel shipmentModel);

        Task<ShipmentModel> GetShipmentById(int id);

        Task<List<ShipmentModel>> GetAll();

        Task Update(int shipmentId,
                                  ShipmentStatus newStatus,
                                  int changedBy,
                                  string? remarks);

        Task Delete(int id);

        Task<object> GetSummary();
    }
}
