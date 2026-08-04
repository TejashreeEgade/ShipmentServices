using Project.Common;
using Project.Model;
using Project.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Project.Repository.Implementation
{
    public class ShipmentModelRepository : IShipmentRepository
    {
        private readonly ShipmentDbContext _context;

        public ShipmentModelRepository(ShipmentDbContext context)
        {
            _context = context;
        }

        public async Task AddShipment(ShipmentModel ShipmentModel)
        {
            await _context.Shipments.AddAsync(ShipmentModel);
        }

        public async Task<ShipmentModel?> GetShipmentById(int id)
        {
            return await _context.Shipments.Include(x => x.statusHistories)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<ShipmentModel> GetAll()
        {
            return _context.Shipments.AsNoTracking();
        }

        public void Update(ShipmentModel ShipmentModel)
        {
            _context.Shipments.Update(ShipmentModel);
        }

        public void Delete(ShipmentModel ShipmentModel)
        {
            _context.Shipments.Remove(ShipmentModel);
        }

        public async Task AddStatusHistory(ShipmentHistoryModel history)
        {
            await _context.ShipmentStatusHistories.AddAsync(history);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

