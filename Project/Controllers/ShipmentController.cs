using Microsoft.AspNetCore.Mvc;
using Project.Enum;
using Project.Model;
using Project.Service.Implementation;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : Controller
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        // POST: api/Shipment
        [HttpPost]
        public async Task<IActionResult> AddShipment([FromBody] ShipmentModel shipmentModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _shipmentService.AddShipment(shipmentModel);

            return Ok(new
            {
                Message = "Shipment created successfully."
            });
        }

        // GET: api/Shipment/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipmentById(int id)
        {
            var shipment = await _shipmentService.GetShipmentById(id);

            if (shipment == null)
                return NotFound(new
                {
                    Message = "Shipment not found."
                });

            return Ok(shipment);
        }

        // GET: api/Shipment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shipments = await _shipmentService.GetAll();

            return Ok(shipments);
        }

        // PUT: api/Shipment/Status
        [HttpPut("Status")]
        public async Task<IActionResult> Update(
            int shipmentId,
            ShipmentStatus newStatus,
            int changedBy,
            string? remarks)
        {
            await _shipmentService.Update(
                shipmentId,
                newStatus,
                changedBy,
                remarks);

            return Ok(new
            {
                Message = "Shipment status updated successfully."
            });
        }

        // DELETE: api/Shipment/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shipmentService.Delete(id);

            return Ok(new
            {
                Message = "Shipment deleted successfully."
            });
        }

        // GET: api/Shipment/Summary
        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _shipmentService.GetSummary();

            return Ok(summary);
        }
    }
}
