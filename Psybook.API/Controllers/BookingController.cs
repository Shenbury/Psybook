using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;
using Psybook.Objects.DbModels;

namespace Psybook.API.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]/[action]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class BookingController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;

    private readonly ILogger<BookingController> _logger;

    public BookingController(ILogger<BookingController> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
            _graphServiceClient = graphServiceClient;
    }

    [HttpGet(Name = "GetCalendarSlots")]
    public async Task<IEnumerable<CalendarSlot>> GetCalendarSlots()
    {
        //var user = await _graphServiceClient.Me.Request().GetAsync();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
