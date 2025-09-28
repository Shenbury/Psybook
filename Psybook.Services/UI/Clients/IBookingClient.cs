using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// Defines contract for booking-related HTTP client operations.
    /// </summary>
    public interface IBookingClient
    {
        /// <summary>
        /// Retrieves all calendar slots from the booking API.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A list of calendar slots, or an empty list if none exist.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<List<CalendarSlot>> GetCalendarSlotsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific calendar slot by ID.
        /// </summary>
        /// <param name="id">The ID of the calendar slot to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The calendar slot if found, null otherwise.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves calendar slots filtered by status.
        /// </summary>
        /// <param name="status">The booking status to filter by.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A list of calendar slots with the specified status.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<List<CalendarSlot>> GetCalendarSlotsByStatusAsync(BookingStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves a calendar slot to the booking API.
        /// </summary>
        /// <param name="calendarSlot">The calendar slot to save.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when calendarSlot is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task SaveCalendarSlotAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing calendar slot.
        /// </summary>
        /// <param name="calendarSlot">The calendar slot to update.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when calendarSlot is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task UpdateCalendarSlotAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the status of a booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="status">The new status.</param>
        /// <param name="reason">Optional reason for the status change.</param>
        /// <param name="modifiedBy">Who made the modification.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus status, string? reason = null, string? modifiedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels a booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to cancel.</param>
        /// <param name="reason">The reason for cancellation.</param>
        /// <param name="modifiedBy">Who cancelled the booking.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<bool> CancelBookingAsync(Guid bookingId, string reason, string? modifiedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirms a pending booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to confirm.</param>
        /// <param name="modifiedBy">Who confirmed the booking.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<bool> ConfirmBookingAsync(Guid bookingId, string? modifiedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves experience information for all booking experiences.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A dictionary mapping booking experiences to their records, or an empty dictionary if none exist.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfoAsync(CancellationToken cancellationToken = default);
    }
}