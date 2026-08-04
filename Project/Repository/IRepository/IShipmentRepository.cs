using Project.Model;

namespace Project.Repository.IRepository
{
    public interface IShipmentRepository
    {
        Task AddShipment(ShipmentModel shipmentModel);
        Task<ShipmentModel> GetShipmentById(int id);
        IQueryable<ShipmentModel> GetAll();

        // Update
        void Update(ShipmentModel shipment);

        // Delete
        void Delete(ShipmentModel shipment);

        // Status History
        Task AddStatusHistory(ShipmentHistoryModel history);

        // Save Changes
        Task SaveChangesAsync();

    }
}
