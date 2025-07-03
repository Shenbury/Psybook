using Psybook.Objects.DbModels;

namespace Psybook.Repositories.Booking
{
    public interface IBookingRepository
    {
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync();
    }
}
