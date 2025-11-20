using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserSubcriptionService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly UserSubcriptionRepository _userSubscriptionRepository;
        private readonly StationRepository _stationRepository;
        private readonly VehicleRepository _vehicleRepository;
        private readonly BatteryRepository _batteryRepository;
        private readonly UserRepository _userRepository;
        private readonly SubcriptionRepository _subcriptionRepository;
        private readonly SwapTransactionRepository _swapTransactionRepository;


        public UserSubcriptionService()
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

        public async Task<UserSubscription?> UpdateUserSubscriptionAsync(UserSubscription userSubscription)
        {
            var existing = await _userSubscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId);
            var plan = await _userSubscriptionRepository.GetByIdAsync(userSubscription.PlanId);
            if (existing == null) return null;
            existing.PlanId = userSubscription.PlanId;
            existing.SwapLimit = plan.SwapLimit;
            existing.EndDate = userSubscription.EndDate;
            existing.Status = userSubscription.Status;
            await _userSubscriptionRepository.UpdateAsync(userSubscription);
            return existing;
        }

        //Dang ki goi
        public async Task<UserSubscription?> RegisterSubscriptionAsync(int userId, int vehicleId, int planId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                throw new Exception("Vehicle not found.");

            var plan = await _userSubscriptionRepository.GetByIdAsync(planId);
            if (plan == null)
                throw new Exception("Subscription plan not found.");

            var existing = await _userSubscriptionRepository.GetActiveByVehicleIdAsync(vehicleId);
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(30);

            UserSubscription saved;

            // 🔁 Nếu xe đã có thuê pin đang hoạt động → gia hạn
            if (existing != null && existing.Status == "Active" && existing.EndDate > DateTime.UtcNow)
            {
                // FIX: EndDate is a non-nullable DateTime, do not use .Value
                existing.EndDate = existing.EndDate.AddDays(30);
                existing.UpdatedAt = DateTime.UtcNow;
                existing.SwapLimit = plan.SwapLimit;
                saved = await _userSubscriptionRepository.UpdateSubscriptionAsync(existing);
            }
            else
            {
                // ➕ Tạo mới subscription với status Pending - chờ thanh toán thành công mới được kích hoạt
                var subscription = new UserSubscription
                {
                    UserId = userId,
                    VehicleId = vehicleId,
                    PlanId = planId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "Confirmed", 
                    SwapLimit = plan.SwapLimit,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                saved = await _userSubscriptionRepository.CreateSubscriptionAsync(subscription);
            }
            return saved;

        }


        // khach hang tra tien thanh cong thi update status thanh Active
        public async Task<bool> UpdateStatusUserSubcription(int userSubcriptionId, string status)
        {
            var userSubcription = await _userSubscriptionRepository.GetByIdAsync(userSubcriptionId);
            if(userSubcription == null)
            {
                throw new Exception("UserSubcription not found.");
            }
            userSubcription.Status = status;
            await _userSubscriptionRepository.UpdateAsync(userSubcription);
            return true;
        }


    }
}
