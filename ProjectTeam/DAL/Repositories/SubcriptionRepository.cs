using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class SubcriptionRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public SubcriptionRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            return await _context.SubscriptionPlans.FirstOrDefaultAsync(s => s.PlanId == id);
        }

        public async Task DeleteAsync(int id)
        {
            var subscription = await _context.SubscriptionPlans.FindAsync(id);
            if (subscription != null)
            {
                _context.SubscriptionPlans.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddAsync(SubscriptionPlan subscription)
        {
            _context.SubscriptionPlans.Add(subscription);
            await _context.SaveChangesAsync();
        }
        public async Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan)
        {
            _context.SubscriptionPlans.Update(subscriptionPlan);
            await _context.SaveChangesAsync();
            return subscriptionPlan;
        }

    }
}
