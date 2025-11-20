using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class SwapTransactionService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly UserSubcriptionRepository _userSubscriptionRepository;
        private readonly StationRepository _stationRepository;
        private readonly VehicleRepository _vehicleRepository;
        private readonly BatteryRepository _batteryRepository;
        private readonly UserRepository _userRepository;
        private readonly SubcriptionRepository _subcriptionRepository;
        private readonly SwapTransactionRepository _swapTransactionRepository;


        public SwapTransactionService()
        {
            _reservationRepository = new ReservationRepository();
            _userRepository = new UserRepository();
            _stationRepository = new StationRepository();
            _vehicleRepository = new VehicleRepository();
            _batteryRepository = new BatteryRepository();
            _swapTransactionRepository = new SwapTransactionRepository();
            _userSubscriptionRepository = new UserSubcriptionRepository();
            _subcriptionRepository = new SubcriptionRepository();

        }



        
        //khi khach hang den lay pin thi tao swap 
        public async Task<string> CreateSwapTransactionByReservationAsync(int reservationId)
        {
            // 1️⃣ Lấy Reservation
            var reservation = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new Exception("Reservation not found");

            if (reservation.Status == "Pending")
            {
                return "Reservation đang chờ xác nhận từ nhân viên.";
            }

            if (reservation.Status == "Completed")
            {
                throw new Exception("Reservation này đã hoàn tất trước đó.");
            }

            // 2️⃣ Lấy thông tin Vehicle
            var vehicle = await _vehicleRepository.GetByIdAsync(reservation.VehicleId)
                ?? throw new Exception("Vehicle not found.");

            // 3️⃣ Lấy pin mới tại trạm
            //var newBattery = await _batteryRepository.GetAvailableBatteryAtStationAsync(reservation.StationId)
            //    ?? throw new Exception("Không có pin khả dụng tại trạm.");

            // 4️⃣ Tạo bản ghi SwapTransaction
            var swap = new SwapTransaction
            {
                ReservationId = reservation.ReservationId,
                UserId = reservation.UserId,
                VehicleId = reservation.VehicleId,
                StationId = reservation.StationId,
                FromBatteryId = vehicle.CurrentBatteryId ?? 0,
                ToBatteryId = reservation.BatteryId ?? 0,
                SwapTime = DateTime.UtcNow,
                Status = "Pending"
            };

            await _swapTransactionRepository.AddAsync(swap);


            var activeSub = await _userSubscriptionRepository.GetActiveByUserIdAsync(reservation.UserId);
            if (activeSub != null && activeSub.SwapLimit.HasValue && activeSub.SwapLimit > 0)
            {
                activeSub.SwapLimit -= 1;
                await _userSubscriptionRepository.UpdateAsync(activeSub);
            }

            reservation.Status = "Completed";
            reservation.UpdatedAt = DateTime.UtcNow;
            await _reservationRepository.UpdateAsync(reservation);

            // 6️⃣ Cập nhật trạng thái pin sau khi đổi
          string fromBatteryStatus = reservation.Status;

                if (swap.FromBattery.Soh >= 80)
                {
                    // Pin còn tốt → đưa đi sạc
                    fromBatteryStatus = "Charging";
                }
                else if (swap.FromBattery.Soh >= 60)
                {
                    // Pin chai nhẹ → cần bảo trì
                    fromBatteryStatus = "Maintenance";
                }
                else
                {
                    // Pin chai nặng → loại khỏi hệ thống
                    fromBatteryStatus = "Decommissioned";
                }

                await _batteryRepository.UpdateStatusAsync(swap.FromBatteryId, fromBatteryStatus);
            

            // Cập nhật trạng thái pin khách nhận
           
                await _batteryRepository.UpdateStatusAsync(swap.ToBatteryId, "InUse");
            




            return "Tạo giao dịch đổi pin thành công.";
        }
      





        // khach hang doi pin thanh cong thi cap nhap lai status 
        public async Task<bool> UpdateStatusByUserAsync(int userId,int swapId, string status)
        {
            var swap = await _swapTransactionRepository.GetBySwapIdAsync(swapId);
            if (swap == null)
            {
                return false;
            }

            swap.StaffId = userId;
            swap.Status = status;
            swap.SwapTime = DateTime.UtcNow;

            await _swapTransactionRepository.UpdateAsync(swap);
            return true;
        }
    }
}
