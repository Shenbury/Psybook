using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using System.Net.Http.Json;

namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// HTTP client for booking-related API operations.
    /// Handles communication with the booking API for calendar slots and experience information.
    /// </summary>
    public sealed class BookingClient : IBookingClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BookingClient> _logger;
        private readonly BookingClientOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingClient"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
        /// <param name="logger">Logger for tracking operations and errors.</param>
        /// <param name="options">Configuration options for the client.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public BookingClient(
            IHttpClientFactory httpClientFactory, 
            ILogger<BookingClient> logger,
            IOptions<BookingClientOptions>? options = null)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? new BookingClientOptions();
        }

        /// <inheritdoc />
        public async Task<List<CalendarSlot>> GetCalendarSlotsAsync(CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetCalendarSlotsInternalAsync(cancellationToken),
                _logger,
                "retrieving calendar slots from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetCalendarSlotByIdInternalAsync(id, cancellationToken),
                _logger,
                $"retrieving calendar slot {id} from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<CalendarSlot>> GetCalendarSlotsByStatusAsync(BookingStatus status, CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetCalendarSlotsByStatusInternalAsync(status, cancellationToken),
                _logger,
                $"retrieving calendar slots with status {status} from API",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task SaveCalendarSlotAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(calendarSlot);
            ValidateCalendarSlot(calendarSlot);

            await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await SaveCalendarSlotInternalAsync(calendarSlot, cancellationToken),
                _logger,
                $"saving calendar slot with ID {calendarSlot.Id}",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task UpdateCalendarSlotAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(calendarSlot);
            ValidateCalendarSlot(calendarSlot);

            await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await UpdateCalendarSlotInternalAsync(calendarSlot, cancellationToken),
                _logger,
                $"updating calendar slot with ID {calendarSlot.Id}",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus status, string? reason = null, string? modifiedBy = null, CancellationToken cancellationToken = default)
        {
            await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await UpdateBookingStatusInternalAsync(bookingId, status, reason, modifiedBy, cancellationToken),
                _logger,
                $"updating booking status for {bookingId} to {status}",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CancelBookingAsync(Guid bookingId, string reason, string? modifiedBy = null, CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await CancelBookingInternalAsync(bookingId, reason, modifiedBy, cancellationToken),
                _logger,
                $"cancelling booking {bookingId}",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> ConfirmBookingAsync(Guid bookingId, string? modifiedBy = null, CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await ConfirmBookingInternalAsync(bookingId, modifiedBy, cancellationToken),
                _logger,
                $"confirming booking {bookingId}",
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfoAsync(CancellationToken cancellationToken = default)
        {
            return await HttpClientExtensions.ExecuteWithErrorHandlingAsync(
                async () => await GetExperienceInfoInternalAsync(cancellationToken),
                _logger,
                "retrieving experience information from API",
                cancellationToken);
        }

        /// <summary>
        /// Internal implementation for retrieving calendar slots.
        /// </summary>
        private async Task<List<CalendarSlot>> GetCalendarSlotsInternalAsync(CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var calendarSlots = new List<CalendarSlot>();
            var itemCount = 0;

            await foreach (var slot in httpClient.GetFromJsonAsAsyncEnumerable<CalendarSlot>(
                BookingClientConstants.ApiRoutes.GetCalendarSlots, 
                cancellationToken))
            {
                if (slot is not null)
                {
                    calendarSlots.Add(slot);
                    itemCount++;

                    if (itemCount >= _options.MaxItemsPerRequest)
                    {
                        _logger.LogWarning("Reached maximum items limit of {MaxItems} for calendar slots", 
                            _options.MaxItemsPerRequest);
                        break;
                    }
                }
            }

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(calendarSlots, _logger, "GetCalendarSlots");
            }

            return calendarSlots;
        }

        /// <summary>
        /// Internal implementation for retrieving a calendar slot by ID.
        /// </summary>
        private async Task<CalendarSlot?> GetCalendarSlotByIdInternalAsync(Guid id, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.GetAsync($"Booking/GetCalendarSlotById/{id}", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CalendarSlot>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Internal implementation for retrieving calendar slots by status.
        /// </summary>
        private async Task<List<CalendarSlot>> GetCalendarSlotsByStatusInternalAsync(BookingStatus status, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var calendarSlots = await httpClient.GetFromJsonAsync<List<CalendarSlot>>(
                $"Booking/GetCalendarSlotsByStatus/{status}", 
                cancellationToken);

            return calendarSlots ?? new List<CalendarSlot>();
        }

        /// <summary>
        /// Internal implementation for saving a calendar slot.
        /// </summary>
        private async Task<object> SaveCalendarSlotInternalAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.PostAsJsonAsync(
                BookingClientConstants.ApiRoutes.SaveCalendarSlot, 
                calendarSlot, 
                cancellationToken);

            response.EnsureSuccessStatusCode();
            
            if (_options.ValidateResponses)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug("Save response: {Response}", responseContent);
            }

            return new object(); // Return something for the extension method
        }

        /// <summary>
        /// Internal implementation for updating a calendar slot.
        /// </summary>
        private async Task<object> UpdateCalendarSlotInternalAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.PutAsJsonAsync(
                "Booking/UpdateCalendarSlot", 
                calendarSlot, 
                cancellationToken);

            response.EnsureSuccessStatusCode();
            return new object();
        }

        /// <summary>
        /// Internal implementation for updating booking status.
        /// </summary>
        private async Task<object> UpdateBookingStatusInternalAsync(Guid bookingId, BookingStatus status, string? reason, string? modifiedBy, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var request = new { Status = status, Reason = reason, ModifiedBy = modifiedBy };
            var response = await httpClient.PatchAsJsonAsync(
                $"Booking/UpdateBookingStatus/{bookingId}/status", 
                request, 
                cancellationToken);

            response.EnsureSuccessStatusCode();
            return new object();
        }

        /// <summary>
        /// Internal implementation for cancelling a booking.
        /// </summary>
        private async Task<bool> CancelBookingInternalAsync(Guid bookingId, string reason, string? modifiedBy, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var request = new { Reason = reason, ModifiedBy = modifiedBy };
            var response = await httpClient.PatchAsJsonAsync(
                $"Booking/CancelBooking/{bookingId}/cancel", 
                request, 
                cancellationToken);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Internal implementation for confirming a booking.
        /// </summary>
        private async Task<bool> ConfirmBookingInternalAsync(Guid bookingId, string? modifiedBy, CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var request = new { ModifiedBy = modifiedBy };
            var response = await httpClient.PatchAsJsonAsync(
                $"Booking/ConfirmBooking/{bookingId}/confirm", 
                request, 
                cancellationToken);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Internal implementation for retrieving experience information.
        /// </summary>
        private async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfoInternalAsync(CancellationToken cancellationToken)
        {
            using var httpClient = CreateConfiguredHttpClient();
            var experienceInfo = await httpClient.GetFromJsonAsync<Dictionary<BookingExperience, ExperienceRecord>>(
                BookingClientConstants.ApiRoutes.GetExperienceInfo, 
                cancellationToken);

            var result = experienceInfo ?? new Dictionary<BookingExperience, ExperienceRecord>();

            if (_options.ValidateResponses)
            {
                HttpClientExtensions.ValidateResponse(experienceInfo, _logger, "GetExperienceInfo");
                
                if (result.Count == 0)
                {
                    _logger.LogWarning("No experience information was returned from the API");
                }
            }

            return result;
        }

        /// <summary>
        /// Creates and configures an HTTP client for booking API operations.
        /// </summary>
        /// <returns>A configured HTTP client.</returns>
        private HttpClient CreateConfiguredHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient(BookingClientConstants.HttpClientName);
            httpClient.Timeout = _options.RequestTimeout;
            return httpClient;
        }

        /// <summary>
        /// Validates a calendar slot before sending it to the API.
        /// </summary>
        /// <param name="calendarSlot">The calendar slot to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the calendar slot is invalid.</exception>
        private static void ValidateCalendarSlot(CalendarSlot calendarSlot)
        {
            if (string.IsNullOrWhiteSpace(calendarSlot.Title))
            {
                throw new ArgumentException("Calendar slot title cannot be null or empty.", nameof(calendarSlot));
            }

            if (string.IsNullOrWhiteSpace(calendarSlot.FirstName))
            {
                throw new ArgumentException("First name cannot be null or empty.", nameof(calendarSlot));
            }

            if (string.IsNullOrWhiteSpace(calendarSlot.LastName))
            {
                throw new ArgumentException("Last name cannot be null or empty.", nameof(calendarSlot));
            }

            if (calendarSlot.Start == default)
            {
                throw new ArgumentException("Start time must be specified.", nameof(calendarSlot));
            }

            if (calendarSlot.End.HasValue && calendarSlot.End <= calendarSlot.Start)
            {
                throw new ArgumentException("End time must be after start time.", nameof(calendarSlot));
            }
        }
    }
}
