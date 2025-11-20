using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class StationRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public StationRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }
        public async Task<IEnumerable<Station>> GetAllAsync()
        {
            return await _context.Stations.ToListAsync();
        }

        public async Task<Station?> GetByIdAsync(int id)
        {
            return await _context.Stations.FirstOrDefaultAsync(s => s.StationId == id);
        }

        public async Task<Station> AddAsync(Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
            return station;
        }



    }
}
