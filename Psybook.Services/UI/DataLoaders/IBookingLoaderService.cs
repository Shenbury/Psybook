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
    /// Retrieves a specific calendar slot by ID.
    /// </summary>
    /// <param name="id">The ID of the calendar slot to retrieve.</param>
    /// <returns>The calendar slot if found, null otherwise.</returns>
    Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id);

    /// <summary>
    /// Saves a calendar slot.
    /// </summary>
    /// <param name="calendarSlot">The calendar slot to save.</param>
    /// <exception cref="ArgumentNullException">Thrown when calendarSlot is null.</exception>
    Task SaveCalendarSlot(CalendarSlot calendarSlot);

    /// <summary>
    /// Updates an existing calendar slot.
    /// </summary>
    /// <param name="calendarSlot">The calendar slot to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when calendarSlot is null.</exception>
    Task UpdateCalendarSlot(CalendarSlot calendarSlot);

    /// <summary>
    /// Updates the status of a booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to update.</param>
    /// <param name="status">The new status.</param>
    /// <param name="reason">Optional reason for the status change.</param>
    /// <param name="modifiedBy">Who made the modification.</param>
    Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus status, string? reason = null, string? modifiedBy = null);

    /// <summary>
    /// Cancels a booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to cancel.</param>
    /// <param name="reason">The reason for cancellation.</param>
    /// <param name="modifiedBy">Who cancelled the booking.</param>
    /// <returns>True if the booking was successfully cancelled, false otherwise.</returns>
    Task<bool> CancelBookingAsync(Guid bookingId, string reason, string? modifiedBy = null);

    /// <summary>
    /// Confirms a pending booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to confirm.</param>
    /// <param name="modifiedBy">Who confirmed the booking.</param>
    /// <returns>True if the booking was successfully confirmed, false otherwise.</returns>
    Task<bool> ConfirmBookingAsync(Guid bookingId, string? modifiedBy = null);

    /// <summary>
    /// Retrieves experience information for all booking experiences.
    /// </summary>
    /// <returns>A dictionary mapping booking experiences to their records, or an empty dictionary if none exist.</returns>
    Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo();
}