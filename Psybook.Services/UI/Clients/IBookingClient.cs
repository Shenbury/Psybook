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
        /// Saves a calendar slot to the booking API.
        /// </summary>
        /// <param name="calendarSlot">The calendar slot to save.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when calendarSlot is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task SaveCalendarSlotAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves experience information for all booking experiences.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A dictionary mapping booking experiences to their records, or an empty dictionary if none exist.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfoAsync(CancellationToken cancellationToken = default);
    }
}