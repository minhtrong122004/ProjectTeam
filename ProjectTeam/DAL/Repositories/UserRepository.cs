using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository
    {
        private readonly EvBatterySwapSystemContext _context;

        public UserRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }
        public async Task<User?> GetUserAsync(string email, string password)
        {
            return await _context.Users.Include(u => u.Role)
                                       .Include(u => u.Vehicle)
                                       .FirstOrDefaultAsync(u => u.Email == email && u.Password == password 
                                       && u.Status == "Active");
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }


    }
}
