using Microsoft.EntityFrameworkCore;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Shared.Contexts;

namespace Psybook.Repositories.Booking
{
    public class SqlBookingRepository: IBookingRepository
    {
        private readonly BookingContext _bookingContext;

        public SqlBookingRepository(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync()
        {
            return await _bookingContext.CalendarSlots.AsNoTracking().ToArrayAsync();
        }

        public async Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id)
        {
            return await _bookingContext.CalendarSlots
                .AsNoTracking()
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsByStatusAsync(BookingStatus status)
        {
            return await _bookingContext.CalendarSlots
                .AsNoTracking()
                .Where(cs => cs.Status == status)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsByExperienceAsync(BookingExperience bookingExperience)
        {
            return await _bookingContext.CalendarSlots
                .AsNoTracking()
                .Where(cs => cs.BookingExperience == bookingExperience)
                .ToArrayAsync();
        }

        public async Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot)
        {
            calendarSlot.CreatedAt = DateTime.UtcNow;
            await _bookingContext.AddAsync(calendarSlot);
            await _bookingContext.SaveChangesAsync();
        }

        public async Task UpdateCalendarSlotAsync(CalendarSlot calendarSlot)
        {
            calendarSlot.ModifiedAt = DateTime.UtcNow;
            _bookingContext.CalendarSlots.Update(calendarSlot);
            await _bookingContext.SaveChangesAsync();
        }

        public async Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus newStatus, string? reason = null, string? modifiedBy = null)
        {
            var booking = await _bookingContext.CalendarSlots.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = newStatus;
                booking.ModifiedAt = DateTime.UtcNow;
                booking.ModifiedBy = modifiedBy;

                if (newStatus == BookingStatus.Cancelled)
                {
                    booking.CancellationReason = reason;
                    booking.CancelledAt = DateTime.UtcNow;
                }

                await _bookingContext.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteCalendarSlotAsync(Guid id)
        {
            var booking = await _bookingContext.CalendarSlots.FindAsync(id);
            if (booking != null)
            {
                _bookingContext.CalendarSlots.Remove(booking);
                await _bookingContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
