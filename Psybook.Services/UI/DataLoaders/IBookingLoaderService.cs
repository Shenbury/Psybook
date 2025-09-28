using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.UI.DataLoaders;

/// <summary>
/// Defines contract for UI-layer booking data loading operations.
/// </summary>
public interface IBookingLoaderService
{
    /// <summary>
    /// Retrieves multiple calendar slots for display in the UI.
    /// </summary>
    /// <returns>A list of calendar slots, or an empty list if none exist.</returns>
    Task<List<CalendarSlot>> GetMultipleCalendarSlots();

    /// <summary>
    /// Saves a calendar slot.
    /// </summary>
    /// <param name="calendarSlot">The calendar slot to save.</param>
    /// <exception cref="ArgumentNullException">Thrown when calendarSlot is null.</exception>
    Task SaveCalendarSlot(CalendarSlot calendarSlot);

    /// <summary>
    /// Retrieves experience information for all booking experiences.
    /// </summary>
    /// <returns>A dictionary mapping booking experiences to their records, or an empty dictionary if none exist.</returns>
    Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo();
}