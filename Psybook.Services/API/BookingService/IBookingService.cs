using Psybook.Objects.DbModels;

namespace Psybook.Services.API.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync();
    }
}
