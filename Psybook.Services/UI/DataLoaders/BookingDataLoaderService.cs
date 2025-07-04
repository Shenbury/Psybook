using Psybook.Objects.DbModels;
using Psybook.Services.UI.Clients;

namespace Psybook.Services.UI.DataLoaders;

public class BookingDataLoaderService(BookingClient _bookingClient) : IBookingLoaderService
{
    public async Task<List<CalendarSlot>> GetMultipleCalendarSlots()
    {
        // Use EF or API to fetch
        var items = await _bookingClient.GetCalendarSlotsAsync();
        return items ?? [];
    }
}