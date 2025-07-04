using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.API.BookingService
{
    public interface IBookingService
    {
        Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync();

        Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot);

        Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo();
    }
}
