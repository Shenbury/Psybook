using Psybook.Objects.DbModels;

namespace Psybook.Services.UI.DataLoaders;
public interface IBookingLoaderService
{
    Task<List<CalendarSlot>> GetMultipleCalendarSlots();
    Task SaveCalendarSlot(CalendarSlot calendarSlot);
}