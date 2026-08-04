using Project.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Model
{
    public class ShipmentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string shipmentNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string customerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string origin { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string destination { get; set; } = string.Empty;

        [Required]
        public ShipmentMode mode { get; set; }

        [Required]
        public ShipmentStatus status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal estimatedRevenue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal estimatedCost { get; set; }

        public int branchId { get; set; }

        public int assignedEmployeeId { get; set; }

        public DateTime expectedDepartureDate { get; set; }

        public DateTime expectedArrivalDate { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime? updatedAt { get; set; }

        // Navigation Property
        public ICollection<ShipmentHistoryModel> statusHistories { get; set; }
            = new List<ShipmentHistoryModel>();
    }
}
