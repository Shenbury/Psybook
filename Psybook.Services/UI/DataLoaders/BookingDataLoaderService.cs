using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.UI.Clients;

namespace Psybook.Services.UI.DataLoaders;

/// <summary>
/// Service for loading booking data from the API for UI components.
/// Handles caching, error handling, and data transformation for the UI layer.
/// </summary>
public sealed class BookingDataLoaderService : IBookingLoaderService
{
    private readonly IBookingClient _bookingClient;
    private readonly ILogger<BookingDataLoaderService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingDataLoaderService"/> class.
    /// </summary>
    /// <param name="bookingClient">The booking client for API operations.</param>
    /// <param name="logger">Logger for tracking operations and errors.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public BookingDataLoaderService(IBookingClient bookingClient, ILogger<BookingDataLoaderService> logger)
    {
        _bookingClient = bookingClient ?? throw new ArgumentNullException(nameof(bookingClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<CalendarSlot>> GetMultipleCalendarSlots()
    {
        try
        {
            _logger.LogInformation("Loading calendar slots for UI display");
            var slots = await _bookingClient.GetCalendarSlotsAsync();
            
            _logger.LogInformation("Successfully loaded {SlotCount} calendar slots", slots.Count);
            return slots;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load calendar slots for UI display");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Loading calendar slot {BookingId} for UI display", id);
            var slot = await _bookingClient.GetCalendarSlotByIdAsync(id);
            
            if (slot != null)
            {
                _logger.LogInformation("Successfully loaded calendar slot {BookingId}", id);
            }
            else
            {
                _logger.LogWarning("Calendar slot {BookingId} not found", id);
            }
            
            return slot;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load calendar slot {BookingId} for UI display", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SaveCalendarSlot(CalendarSlot calendarSlot)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(calendarSlot);
            
            _logger.LogInformation("Saving calendar slot {BookingId} from UI", calendarSlot.Id);
            await _bookingClient.SaveCalendarSlotAsync(calendarSlot);
            
            _logger.LogInformation("Successfully saved calendar slot {BookingId}", calendarSlot.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save calendar slot {BookingId} from UI", calendarSlot?.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateCalendarSlot(CalendarSlot calendarSlot)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(calendarSlot);
            
            _logger.LogInformation("Updating calendar slot {BookingId} from UI", calendarSlot.Id);
            await _bookingClient.UpdateCalendarSlotAsync(calendarSlot);
            
            _logger.LogInformation("Successfully updated calendar slot {BookingId}", calendarSlot.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar slot {BookingId} from UI", calendarSlot?.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus status, string? reason = null, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Updating booking status for {BookingId} to {Status} from UI", bookingId, status);
            await _bookingClient.UpdateBookingStatusAsync(bookingId, status, reason, modifiedBy);
            
            _logger.LogInformation("Successfully updated booking status for {BookingId} to {Status}", bookingId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update booking status for {BookingId} to {Status} from UI", bookingId, status);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CancelBookingAsync(Guid bookingId, string reason, string? modifiedBy = null)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reason, nameof(reason));
            
            _logger.LogInformation("Cancelling booking {BookingId} from UI with reason: {Reason}", bookingId, reason);
            var success = await _bookingClient.CancelBookingAsync(bookingId, reason, modifiedBy);
            
            if (success)
            {
                _logger.LogInformation("Successfully cancelled booking {BookingId}", bookingId);
            }
            else
            {
                _logger.LogWarning("Failed to cancel booking {BookingId} - booking may not exist or cannot be cancelled", bookingId);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel booking {BookingId} from UI", bookingId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ConfirmBookingAsync(Guid bookingId, string? modifiedBy = null)
    {
        try
        {
            _logger.LogInformation("Confirming booking {BookingId} from UI", bookingId);
            var success = await _bookingClient.ConfirmBookingAsync(bookingId, modifiedBy);
            
            if (success)
            {
                _logger.LogInformation("Successfully confirmed booking {BookingId}", bookingId);
            }
            else
            {
                _logger.LogWarning("Failed to confirm booking {BookingId} - booking may not exist or cannot be confirmed", bookingId);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm booking {BookingId} from UI", bookingId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
    {
        try
        {
            _logger.LogInformation("Loading experience information for UI display");
            var experienceInfo = await _bookingClient.GetExperienceInfoAsync();
            
            _logger.LogInformation("Successfully loaded experience information for {ExperienceCount} experiences", 
                experienceInfo.Count);
            return experienceInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load experience information for UI display");
            throw;
        }
    }
}