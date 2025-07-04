using Psybook.Objects.DbModels;
using Psybook.Services.UI.Clients;

namespace Psybook.Services.UI.DataLoaders;
public interface IBookingLoaderService
{
    Task<List<CalendarSlot>> GetMultipleCalendarSlots();
}