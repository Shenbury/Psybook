using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.UI.Clients;

namespace Psybook.Services.UI.DataLoaders;

/// <summary>
/// Service for loading booking-related data.
/// Acts as a facade over the BookingClient for UI-specific operations.
/// </summary>
public sealed class BookingDataLoaderService : IBookingLoaderService
{
    private readonly IBookingClient _bookingClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingDataLoaderService"/> class.
    /// </summary>
    /// <param name="bookingClient">The booking client for API operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when bookingClient is null.</exception>
    public BookingDataLoaderService(IBookingClient bookingClient)
    {
        _bookingClient = bookingClient ?? throw new ArgumentNullException(nameof(bookingClient));
    }

    /// <inheritdoc />
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
    {
        return await _bookingClient.GetExperienceInfoAsync();
    }

    /// <inheritdoc />
    public async Task<List<CalendarSlot>> GetMultipleCalendarSlots()
    {
        var items = await _bookingClient.GetCalendarSlotsAsync();
        return items ?? [];
    }

    /// <inheritdoc />
    public async Task SaveCalendarSlot(CalendarSlot calendarSlot)
    {
        await _bookingClient.SaveCalendarSlotAsync(calendarSlot);
    }
}