using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ReservationRepository
    {
        private readonly EvBatterySwapSystemContext _context;
        public ReservationRepository()
        {
            _context = new EvBatterySwapSystemContext();
        }

        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

        }



        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Battery)
                .Include(r => r.Station)
                .Include(r => r.Vehicle)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }
        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }
        public async Task<List<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Reservation>> GetByStationIdAsync(int stationId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .Include(r => r.Battery)
                .Where(r => r.StationId == stationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetConfirmedReservationsPastEndTimeAsync(int userId, int vehicleId, DateTime currentTime)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId
                    && r.VehicleId == vehicleId
                    && r.Status == "Confirmed"
                    && r.EndTime < currentTime)
        .ToListAsync();
        }
    }
}

