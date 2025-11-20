using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class VehicleRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public VehicleRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();

        }

        public async Task<Vehicle> AddAsyncc(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<List<Vehicle>> GetListByUserIdAsync(int userId)
        {
            return await _context.Vehicles
                .Include(v => v.Model)
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }
        public async Task<Vehicle?> GetByUserAndIdAsync(int userId, int vehicleId)
        {
            return await _context.Vehicles
                          .Include(v => v.User) // lấy thêm thông tin người dùng
                          .FirstOrDefaultAsync(v => v.VehicleId == vehicleId && v.UserId == userId);
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles.Include(v => v.User).FirstOrDefaultAsync(v => v.VehicleId == id);

        }

        public async Task<Vehicle?> GetByVinAsync(string vin)
        {
            return await _context.Vehicles.Include(v => v.User).FirstOrDefaultAsync(v => v.Vin == vin);

        }
        public async Task AddAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }


    }
}
