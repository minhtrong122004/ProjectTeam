using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public async Task<User?> GetUserAsync(string email, string password)
        {
            return await _userRepository.GetUserAsync(email, password);
        }
    }
}
