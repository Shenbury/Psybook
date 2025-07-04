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

    [HttpPost(Name = "SaveCalendarSlot")]
    public async Task SaveCalendarSlots(CalendarSlot calendarSlot)
    {
        await _bookingService.SaveCalendarSlotsAsync(calendarSlot);
    }

    [HttpGet(Name = "GetExperienceInfo")]
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
    {
        return await _bookingService.GetExperienceInfo();
    }
}
