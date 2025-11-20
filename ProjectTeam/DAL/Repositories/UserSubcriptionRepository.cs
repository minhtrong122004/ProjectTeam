using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserSubcriptionRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public UserSubcriptionRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }

        public async Task<UserSubscription?> GetActiveByUserIdAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.Plan)
                .Include(us => us.Vehicle)
                .Where(us => us.UserId == userId && us.Status == "Active")
                .OrderByDescending(us => us.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserSubscription?>> GetByUserIdAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.Plan)
                .Include(us => us.Vehicle)
                .Include(us => us.User)
                .Where(us => us.UserId == userId)
                .ToListAsync();

        }
        public async Task<UserSubscription?> GetActiveByVehicleIdAsync(int vehicleId)
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Include(u => u.Vehicle)
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.VehicleId == vehicleId && u.Status == "Active");
        }

        public async Task<IEnumerable<UserSubscription>> GetAllAsync()
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Include(u => u.Vehicle)
                .Include(u => u.User)
                .ToListAsync();
        }

        public async Task<UserSubscription?> GetByIdAsync(int id)
        {
            return await _context.UserSubscriptions
              .FirstOrDefaultAsync(us => us.SubscriptionId == id);
        }

        public async Task UpdateAsync(UserSubscription userSubscription)
        {
            _context.UserSubscriptions.Update(userSubscription);
            await _context.SaveChangesAsync();
        }
        public async Task<UserSubscription> CreateSubscriptionAsync(UserSubscription subscription)
        {
            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }
        public async Task<UserSubscription> UpdateSubscriptionAsync(UserSubscription subscription)
        {
            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }
        public async Task<UserSubscription?> GetActiveSubscriptionAsync(int userId, int vehicleId)
        {
            return await _context.UserSubscriptions.FirstOrDefaultAsync(us => us.UserId == userId
                                                                        && us.VehicleId == vehicleId
                                                                        && us.Status == "Active"
                                                                        && us.StartDate <= DateTime.Now
                                                                        && us.EndDate >= DateTime.Now);

        }
    }
}
