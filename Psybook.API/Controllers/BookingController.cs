using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.API.BookingService;

namespace Psybook.API.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]/[action]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class BookingController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(ILogger<BookingController> logger, GraphServiceClient graphServiceClient, IBookingService bookingService)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
        _bookingService = bookingService;
    }

    [HttpGet(Name = "GetCalendarSlots")]
    public async Task<IEnumerable<CalendarSlot>> GetCalendarSlots()
    {
        //var user = await _graphServiceClient.Me.Request().GetAsync();
        return await _bookingService.GetCalendarSlotsAsync();
    }

    [HttpGet("{id:guid}", Name = "GetCalendarSlotById")]
    public async Task<ActionResult<CalendarSlot>> GetCalendarSlotById(Guid id)
    {
        var slot = await _bookingService.GetCalendarSlotByIdAsync(id);
        if (slot == null)
        {
            return NotFound();
        }
        return Ok(slot);
    }

    [HttpGet("{status}", Name = "GetCalendarSlotsByStatus")]
    public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsByStatus(BookingStatus status)
    {
        return await _bookingService.GetCalendarSlotsByStatusAsync(status);
    }

    [HttpPost(Name = "SaveCalendarSlot")]
    public async Task SaveCalendarSlot(CalendarSlot calendarSlot)
    {
        await _bookingService.SaveCalendarSlotsAsync(calendarSlot);
    }

    [HttpPut(Name = "UpdateCalendarSlot")]
    public async Task<IActionResult> UpdateCalendarSlot(CalendarSlot calendarSlot)
    {
        try
        {
            await _bookingService.UpdateCalendarSlotAsync(calendarSlot);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar slot {BookingId}", calendarSlot.Id);
            return BadRequest("Failed to update booking");
        }
    }

    [HttpPatch("{bookingId:guid}/status", Name = "UpdateBookingStatus")]
    public async Task<IActionResult> UpdateBookingStatus(Guid bookingId, [FromBody] UpdateBookingStatusRequest request)
    {
        try
        {
            await _bookingService.UpdateBookingStatusAsync(bookingId, request.Status, request.Reason, request.ModifiedBy);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update booking status for {BookingId}", bookingId);
            return BadRequest("Failed to update booking status");
        }
    }

    [HttpPatch("{bookingId:guid}/cancel", Name = "CancelBooking")]
    public async Task<IActionResult> CancelBooking(Guid bookingId, [FromBody] CancelBookingRequest request)
    {
        try
        {
            var success = await _bookingService.CancelBookingAsync(bookingId, request.Reason, request.ModifiedBy);
            if (!success)
            {
                return BadRequest("Booking cannot be cancelled or does not exist");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel booking {BookingId}", bookingId);
            return BadRequest("Failed to cancel booking");
        }
    }

    [HttpPatch("{bookingId:guid}/confirm", Name = "ConfirmBooking")]
    public async Task<IActionResult> ConfirmBooking(Guid bookingId, [FromBody] ConfirmBookingRequest? request = null)
    {
        try
        {
            var success = await _bookingService.ConfirmBookingAsync(bookingId, request?.ModifiedBy);
            if (!success)
            {
                return BadRequest("Booking cannot be confirmed or does not exist");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm booking {BookingId}", bookingId);
            return BadRequest("Failed to confirm booking");
        }
    }

    [HttpGet(Name = "GetExperienceInfo")]
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
    {
        return await _bookingService.GetExperienceInfo();
    }
}

// Request DTOs
public record UpdateBookingStatusRequest(BookingStatus Status, string? Reason = null, string? ModifiedBy = null);
public record CancelBookingRequest(string Reason, string? ModifiedBy = null);
public record ConfirmBookingRequest(string? ModifiedBy = null);
