using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.UI.DataLoaders;
public interface IBookingLoaderService
{
    Task<List<CalendarSlot>> GetMultipleCalendarSlots();
    Task SaveCalendarSlot(CalendarSlot calendarSlot);
    Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo();
}