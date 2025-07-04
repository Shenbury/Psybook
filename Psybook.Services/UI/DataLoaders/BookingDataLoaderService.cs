using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.UI.Clients;

namespace Psybook.Services.UI.DataLoaders;

public class BookingDataLoaderService(BookingClient _bookingClient) : IBookingLoaderService
{
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
    {
        return await _bookingClient.GetExperienceInfoAsync();
    }

    public async Task<List<CalendarSlot>> GetMultipleCalendarSlots()
    {
        // Use EF or API to fetch
        var items = await _bookingClient.GetCalendarSlotsAsync();
        return items ?? [];
    }

    public async Task SaveCalendarSlot(CalendarSlot calendarSlot)
    {
        await _bookingClient.SaveCalendarSlotsAsync(calendarSlot);
    }
}