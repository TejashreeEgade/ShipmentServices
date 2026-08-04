using Project.Enum;

namespace Project.Model
{
    public class ShipmentHistoryModel
    {
        public int Id { get; set; }
        public int shipmentId { get; set; }
        public ShipmentStatus previosStatus { get; set; }
        public ShipmentStatus newStatus { get; set; }
        public int changedBy { get; set; }
        public string? remarks { get; set; }
        public DateTime changedAt { get; set; }
        public ShipmentModel shipment { get; set; } = null;
    }
}
