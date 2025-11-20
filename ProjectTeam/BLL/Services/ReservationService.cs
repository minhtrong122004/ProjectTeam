using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReservationService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly UserSubcriptionRepository _userSubscriptionRepository;
        private readonly StationRepository _stationRepository;
        private readonly VehicleRepository _vehicleRepository;
        private readonly BatteryRepository _batteryRepository;
        private readonly UserRepository _userRepository;


        public ReservationService()
        {
            _reservationRepository = new ReservationRepository();
            _userRepository = new UserRepository();
            _stationRepository = new StationRepository();
            _vehicleRepository = new VehicleRepository();
            _batteryRepository = new BatteryRepository();
            _userSubscriptionRepository = new UserSubcriptionRepository();

        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)

        {

            await _reservationRepository.AddAsync(reservation);
            return reservation;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            var existing = await _reservationRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _reservationRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _reservationRepository.GetAllAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _reservationRepository.GetByIdAsync(id);
        }



        public async Task<Reservation?> UpdateReservationAsync(Reservation reservation)
        {

            var existing = await _reservationRepository.GetByIdAsync(reservation.ReservationId);
            if (existing == null) return null;
            existing.UserId = reservation.UserId;
            existing.Status = reservation.Status;
            existing.VehicleId = reservation.VehicleId;
            existing.StationId = reservation.StationId;
            existing.StartTime = reservation.StartTime;
            existing.EndTime = reservation.EndTime;

            await _reservationRepository.UpdateAsync(reservation);
            return existing;
        }





        //DUNG DAT LICH DOI PIN
        public async Task<Reservation?> BookReservationAsync(int UserId, int VehicleId, int StationId, DateTime StartTime)
        {

            var overdueReservations = await _reservationRepository
                .GetConfirmedReservationsPastEndTimeAsync(UserId, VehicleId, DateTime.UtcNow);

            foreach (var r in overdueReservations)
            {
                r.Status = "Cancelled";

                await _reservationRepository.UpdateAsync(r);
            }
            // Validate user
            var user = await _userRepository.GetByIdAsync(UserId);
            if (user == null)
            {
                throw new Exception("User does not exist.");
            }

            // Validate station
            var station = await _stationRepository.GetByIdAsync(StationId);
            if (station == null)
            {
                throw new Exception("Station does not exist.");
            }

            // Validate vehicle belongs to the user
            var vehicle = await _vehicleRepository.GetByUserAndIdAsync(UserId, VehicleId);
            if (vehicle == null)
            {
                throw new Exception("Vehicle does not exist or does not belong to the user.");
            }

            // Validate active subscription
            var userSubscription = await _userSubscriptionRepository.GetActiveSubscriptionAsync(UserId,VehicleId);
            if (userSubscription == null)
            {
                throw new Exception("User does not have an active subscription.");
            }


            // Check remaining swap limit
            if (userSubscription.SwapLimit <= 0)
            {
                throw new Exception("User has no remaining swaps.");
            }

            // Create reservation
            var reservation = new Reservation
            {
                UserId = UserId,
                VehicleId = VehicleId,
                StationId = StationId,
                BatteryId = null,
                StartTime = StartTime,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            await _reservationRepository.AddAsync(reservation);
            return reservation;
        }


        //STAFF DUNG CAI NAY DE CAP NHAP LAI TRANG THAI CUA DAT LICH
        public async Task<bool> UpdateStatusReservationAsync(int reservationId, string status)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found.");

            // Cập nhật trạng thái
            reservation.Status = status;


            // Nếu xác nhận, giới hạn trong vòng 12 tiếng
            if (status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
            {
                reservation.StartTime = DateTime.UtcNow;
                reservation.EndTime = DateTime.UtcNow.AddHours(12); //  Giới hạn trong 12 tiếng
            }

            // Nếu hủy thì kết thúc ngay
            else if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                reservation.EndTime = DateTime.UtcNow;
            }


            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }



        // Dung de hien lich su dat lich
        public async Task<List<Reservation>> GetReservationsByUserIdAsync(int userId)
        {
            var reservations = await _reservationRepository.GetByUserIdAsync(userId);

           return reservations;
        }
    }
}
