using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class BatteryService
    {
        private readonly BatteryRepository _batteryRepository;

        public BatteryService()
        {
            _batteryRepository = new BatteryRepository();
        }


        //coi tat ca pin trong kho
        public async Task<IEnumerable<Battery>> GetAllAsync()
        {
            var batteries = await _batteryRepository.GetAllAsync();
            return batteries;
        }



         // lay tat ca nhung pin da san sang de thay cho khach
        public async Task<IEnumerable<Battery?>> GetAvailableBatteryAsync(int vehicleId)
        {
            var batteries = await _batteryRepository.GetAvailableByBatteryAsync(vehicleId);
            if(batteries == null)
            {
                throw new Exception("Het pin");
            }
            return batteries;
        }
    }
}
