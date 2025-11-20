using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class BatteryRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public BatteryRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }

        public async Task<IEnumerable<Battery>> GetAvailableByBatteryAsync(int vehicleId)
        {
            // Load vehicle with its model
            var vehicle = await _context.Vehicles
                .Include(v => v.Model)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null || vehicle.Model == null)
                return Enumerable.Empty<Battery>();

            // Use the vehicle's ModelId to find matching batteries
            var requiredModelId = vehicle.ModelId;

            // Query available batteries matching the model and soh threshold
            return await _context.Batteries
                .Where(b => b.ModelId == requiredModelId
                            && b.Status == "Available"
                            )
                .ToListAsync();
        }
      
        public async Task UpdateStatusAsync(int batteryId, string status)
        {
            var battery = await _context.Batteries.FindAsync(batteryId);
            if (battery == null)
            {
                throw new Exception($"Battery with ID {batteryId} not found.");
            }

            battery.Status = status;
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
        }
        public async Task<Battery> AddAsync(Battery battery)
        {
            _context.Batteries.Add(battery);
            await _context.SaveChangesAsync();
            return battery;
        }

        public async Task<IEnumerable<Battery>> GetAllAsync()
        {
            return await _context.Batteries.ToListAsync();
        }
       
    }
}
