using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.API.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync();
        Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id);
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsByStatusAsync(BookingStatus status);
        Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot);
        Task UpdateCalendarSlotAsync(CalendarSlot calendarSlot);
        Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus newStatus, string? reason = null, string? modifiedBy = null);
        Task<bool> CancelBookingAsync(Guid bookingId, string reason, string? modifiedBy = null);
        Task<bool> ConfirmBookingAsync(Guid bookingId, string? modifiedBy = null);
        Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo();
    }
}
