using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VehicleService
    {
        private readonly VehicleRepository _vehicleRepository;
        private readonly UserRepository _userRepository;
        public VehicleService()
        {
            _vehicleRepository = new VehicleRepository();
            _userRepository = new UserRepository();
        }


        //Dang ki phuong tien
        public async Task<Vehicle?> RegisterVehicleAsync(int userId, int modelId, string vin)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User does not exist.");

            // Kiểm tra VIN đã tồn tại chưa
            var existing = await _vehicleRepository.GetByVinAsync(vin);
            if (existing != null)
                throw new Exception("A vehicle with this VIN already exists.");

            // Tạo mới xe
            var vehicle = new Vehicle
            {
                UserId = userId,
                Vin = vin.ToUpper(), // chuẩn hóa
                ModelId = modelId,
                CurrentBatteryId = null
            };

            await _vehicleRepository.AddAsync(vehicle);
            return vehicle;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Vehicle not found.");
            await _vehicleRepository.DeleteAsync(id);
            return true;
        }
    }
}
