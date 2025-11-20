using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class SwapTransactionRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public SwapTransactionRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }
        public async Task<IEnumerable<SwapTransaction>> GetAllAsync()
        {
            return await _context.SwapTransactions
                .Include(r => r.User)
                .Include(s => s.Station)
                .Include(s => s.Vehicle)
                .Include(s => s.FromBattery)
                .Include(s => s.ToBattery)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.SwapTime)
                .ToListAsync();
        }

        public async Task<SwapTransaction?> GetByIdAsync(int id)
        {
            return await _context.SwapTransactions.FirstOrDefaultAsync(s => s.SwapId == id);
        }

        public async Task<SwapTransaction> AddAsync(SwapTransaction swapTransaction)
        {
            _context.SwapTransactions.Add(swapTransaction);
            await _context.SaveChangesAsync();
            return swapTransaction;
        }

        public async Task<SwapTransaction> UpdateAsync(SwapTransaction swapTransaction)
        {
            _context.SwapTransactions.Update(swapTransaction);
            await _context.SaveChangesAsync();
            return swapTransaction;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var swapTransaction = await _context.SwapTransactions.FindAsync(id);
            if (swapTransaction == null) return false;

            _context.SwapTransactions.Remove(swapTransaction);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<SwapTransaction>> GetByUserIdAsync(int userId)
        {
            return await _context.SwapTransactions
                .Include(r => r.User)
                .Include(s => s.Station)
                .Include(s => s.Vehicle)
                .Include(s => s.FromBattery)
                .Include(s => s.ToBattery)
                .Include(s => s.Payments)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SwapTime)
                .ToListAsync();
        }
        public async Task<SwapTransaction?> GetBySwapIdAsync(int swapId)
        {
            return await _context.SwapTransactions
                .FirstOrDefaultAsync(s => s.SwapId == swapId);
        }
        public async Task<SwapTransaction?> GetByReservationIdAsync(int reservationId)
        {
            return await _context.SwapTransactions
                .FirstOrDefaultAsync(x => x.ReservationId == reservationId);
        }
    }
}
