using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Repositories.Booking
{
    public interface IBookingRepository
    {
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync();
        Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id);
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsByStatusAsync(BookingStatus status);
        Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot);
        Task UpdateCalendarSlotAsync(CalendarSlot calendarSlot);
        Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus newStatus, string? reason = null, string? modifiedBy = null);
        Task<bool> DeleteCalendarSlotAsync(Guid id);
    }
}
