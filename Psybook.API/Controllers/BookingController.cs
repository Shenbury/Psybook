using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.API.BookingService;

namespace Psybook.API.Controllers;

/// <summary>
/// Controller for managing VIP experience bookings
/// </summary>
[ApiController]
[AllowAnonymous] // Temporarily allow anonymous access for development
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all calendar slots
    /// </summary>
    /// <returns>List of calendar slots</returns>
    [HttpGet("slots")]
    public async Task<ActionResult<IEnumerable<CalendarSlot>>> GetSlots()
    {
        try
        {
            var slots = await _bookingService.GetCalendarSlotsAsync();
            return Ok(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving calendar slots");
            return StatusCode(500, "An error occurred while retrieving calendar slots");
        }
    }

    /// <summary>
    /// Get calendar slots by status
    /// </summary>
    /// <param name="status">Booking status to filter by</param>
    /// <returns>List of calendar slots with specified status</returns>
    [HttpGet("slots/status/{status}")]
    public async Task<ActionResult<IEnumerable<CalendarSlot>>> GetSlotsByStatus(BookingStatus status)
    {
        try
        {
            var slots = await _bookingService.GetCalendarSlotsByStatusAsync(status);
            return Ok(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving calendar slots with status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving calendar slots");
        }
    }

    /// <summary>
    /// Get a specific calendar slot by ID
    /// </summary>
    /// <param name="id">Slot ID</param>
    /// <returns>Calendar slot details</returns>
    [HttpGet("slot/{id}")]
    public async Task<ActionResult<CalendarSlot>> GetSlot(Guid id)
    {
        try
        {
            var slot = await _bookingService.GetCalendarSlotByIdAsync(id);
            if (slot == null)
            {
                return NotFound($"Calendar slot with ID {id} not found");
            }
            return Ok(slot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving calendar slot {SlotId}", id);
            return StatusCode(500, "An error occurred while retrieving the calendar slot");
        }
    }

    /// <summary>
    /// Create a new booking/calendar slot
    /// </summary>
    /// <param name="slot">Calendar slot to create</param>
    /// <returns>Created calendar slot</returns>
    [HttpPost("slot")]
    public async Task<ActionResult<CalendarSlot>> CreateSlot([FromBody] CalendarSlot slot)
    {
        if (slot == null)
        {
            return BadRequest("Calendar slot data is required");
        }

        try
        {
            // Validate the slot
            if (slot.Start >= slot.End)
            {
                return BadRequest("Start time must be before end time");
            }

            if (slot.Start <= DateTime.UtcNow)
            {
                return BadRequest("Start time must be in the future");
            }

            await _bookingService.SaveCalendarSlotsAsync(slot);
            return CreatedAtAction(nameof(GetSlot), new { id = slot.Id }, slot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating calendar slot");
            return StatusCode(500, "An error occurred while creating the calendar slot");
        }
    }

    /// <summary>
    /// Update an existing calendar slot
    /// </summary>
    /// <param name="id">Slot ID to update</param>
    /// <param name="slot">Updated slot data</param>
    /// <returns>Updated calendar slot</returns>
    [HttpPut("slot/{id}")]
    public async Task<ActionResult<CalendarSlot>> UpdateSlot(Guid id, [FromBody] CalendarSlot slot)
    {
        if (slot == null)
        {
            return BadRequest("Calendar slot data is required");
        }

        if (id != slot.Id)
        {
            return BadRequest("Slot ID mismatch");
        }

        try
        {
            var existingSlot = await _bookingService.GetCalendarSlotByIdAsync(id);
            if (existingSlot == null)
            {
                return NotFound($"Calendar slot with ID {id} not found");
            }

            // Validate the updated slot
            if (slot.Start >= slot.End)
            {
                return BadRequest("Start time must be before end time");
            }

            await _bookingService.UpdateCalendarSlotAsync(slot);
            return Ok(slot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating calendar slot {SlotId}", id);
            return StatusCode(500, "An error occurred while updating the calendar slot");
        }
    }

    /// <summary>
    /// Update booking status
    /// </summary>
    /// <param name="id">Slot ID</param>
    /// <param name="request">Status update request</param>
    /// <returns>Success status</returns>
    [HttpPost("slot/{id}/status")]
    public async Task<ActionResult> UpdateBookingStatus(Guid id, [FromBody] BookingStatusUpdateRequest request)
    {
        try
        {
            var existingSlot = await _bookingService.GetCalendarSlotByIdAsync(id);
            if (existingSlot == null)
            {
                return NotFound($"Calendar slot with ID {id} not found");
            }

            var currentUser = User.Identity?.Name ?? "System";
            await _bookingService.UpdateBookingStatusAsync(id, request.Status, request.Reason, currentUser);
            
            return Ok(new { Message = "Booking status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking status for slot {SlotId}", id);
            return StatusCode(500, "An error occurred while updating the booking status");
        }
    }

    /// <summary>
    /// Confirm a booking
    /// </summary>
    /// <param name="id">Booking ID to confirm</param>
    /// <returns>Success status</returns>
    [HttpPost("slot/{id}/confirm")]
    public async Task<ActionResult> ConfirmBooking(Guid id)
    {
        try
        {
            var currentUser = User.Identity?.Name ?? "System";
            var confirmed = await _bookingService.ConfirmBookingAsync(id, currentUser);
            
            if (confirmed)
            {
                return Ok(new { Message = "Booking confirmed successfully" });
            }
            else
            {
                return BadRequest("Unable to confirm booking");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking {BookingId}", id);
            return StatusCode(500, "An error occurred while confirming the booking");
        }
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    /// <param name="id">Booking ID to cancel</param>
    /// <param name="request">Cancellation request with reason</param>
    /// <returns>Success status</returns>
    [HttpPost("slot/{id}/cancel")]
    public async Task<ActionResult> CancelBooking(Guid id, [FromBody] BookingCancellationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest("Cancellation reason is required");
            }

            var currentUser = User.Identity?.Name ?? "System";
            var cancelled = await _bookingService.CancelBookingAsync(id, request.Reason, currentUser);
            
            if (cancelled)
            {
                return Ok(new { Message = "Booking cancelled successfully" });
            }
            else
            {
                return BadRequest("Unable to cancel booking");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return StatusCode(500, "An error occurred while cancelling the booking");
        }
    }

    /// <summary>
    /// Get experience information
    /// </summary>
    /// <returns>Dictionary of experience information</returns>
    [HttpGet("experiences")]
    public async Task<ActionResult<Dictionary<BookingExperience, ExperienceRecord>>> GetExperiences()
    {
        try
        {
            var experiences = await _bookingService.GetExperienceInfo();
            return Ok(experiences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving experience information");
            return StatusCode(500, "An error occurred while retrieving experience information");
        }
    }

    /// <summary>
    /// Create sample test data for development (Development only)
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("test-data")]
    public async Task<ActionResult> CreateTestData()
    {
        try
        {
            _logger.LogInformation("Creating sample test data for development");

            // Create sample calendar slots
            var sampleSlots = new List<CalendarSlot>
            {
                new CalendarSlot
                {
                    Id = Guid.NewGuid(),
                    Title = "VIP Rhino Experience - Johnson Family",
                    Location = "Rhino Habitat",
                    Color = MudBlazor.Color.Primary,
                    FirstName = "John",
                    LastName = "Johnson",
                    ContactNumber = "01234 567890",
                    FirstLineAddress = "123 Main Street",
                    Postcode = "B1 1AA",
                    BookingExperience = BookingExperience.RhinoKeeper,
                    Status = BookingStatus.Confirmed,
                    Start = DateTime.UtcNow.AddDays(1).Date.AddHours(10), // Tomorrow at 10 AM
                    End = DateTime.UtcNow.AddDays(1).Date.AddHours(12),   // Tomorrow at 12 PM
                    CreatedAt = DateTime.UtcNow,
                    Notes = "Family of 4, special dietary requirements"
                },
                new CalendarSlot
                {
                    Id = Guid.NewGuid(),
                    Title = "Lion Keeper Experience - Smith Party",
                    Location = "Lion Enclosure",
                    Color = MudBlazor.Color.Warning,
                    FirstName = "Sarah",
                    LastName = "Smith",
                    ContactNumber = "01234 567891",
                    FirstLineAddress = "456 Oak Avenue",
                    Postcode = "B2 2BB",
                    BookingExperience = BookingExperience.BigCatKeeper,
                    Status = BookingStatus.Pending,
                    Start = DateTime.UtcNow.AddDays(3).Date.AddHours(14), // 3 days from now at 2 PM
                    End = DateTime.UtcNow.AddDays(3).Date.AddHours(16),   // 3 days from now at 4 PM
                    CreatedAt = DateTime.UtcNow,
                    Notes = "Birthday celebration"
                },
                new CalendarSlot
                {
                    Id = Guid.NewGuid(),
                    Title = "Elephant Keeper Experience - Brown Group",
                    Location = "Elephant Valley",
                    Color = MudBlazor.Color.Secondary,
                    FirstName = "Michael",
                    LastName = "Brown",
                    ContactNumber = "01234 567892",
                    FirstLineAddress = "789 Pine Road",
                    Postcode = "B3 3CC",
                    BookingExperience = BookingExperience.ElephantKeeper,
                    Status = BookingStatus.Confirmed,
                    Start = DateTime.UtcNow.AddDays(7).Date.AddHours(11), // Next week at 11 AM
                    End = DateTime.UtcNow.AddDays(7).Date.AddHours(13),   // Next week at 1 PM
                    CreatedAt = DateTime.UtcNow,
                    Notes = "Corporate team building event"
                }
            };

            // Save the sample slots
            foreach (var slot in sampleSlots)
            {
                await _bookingService.SaveCalendarSlotsAsync(slot);
            }

            _logger.LogInformation("Successfully created {Count} sample calendar slots", sampleSlots.Count);
            
            return Ok(new { 
                Message = $"Successfully created {sampleSlots.Count} sample calendar slots for development",
                CreatedSlots = sampleSlots.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test data");
            return StatusCode(500, "An error occurred while creating test data");
        }
    }
}

/// <summary>
/// Request model for booking status updates
/// </summary>
public class BookingStatusUpdateRequest
{
    public BookingStatus Status { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Request model for booking cancellations
/// </summary>
public class BookingCancellationRequest
{
    public required string Reason { get; set; }
}
