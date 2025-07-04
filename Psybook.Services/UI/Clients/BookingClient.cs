using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using System.Net.Http.Json;

namespace Psybook.Services.UI.Clients
{
    public class BookingClient(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<List<CalendarSlot>> GetCalendarSlotsAsync(
            CancellationToken cancellationToken = default)
        {
            List<CalendarSlot>? calendarSlots = null;

            var httpClient = _httpClientFactory.CreateClient("psybook-api");

            await foreach (var slot in
                httpClient.GetFromJsonAsAsyncEnumerable<CalendarSlot>(
                    "/Booking/GetCalendarSlots", cancellationToken))
            {
                if (slot is not null)
                {
                    calendarSlots ??= [];
                    calendarSlots.Add(slot);
                }
            }

            return calendarSlots ?? [];
        }

        public async Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient("psybook-api");

            await httpClient.PostAsJsonAsync("/Booking/SaveCalendarSlot", calendarSlot, cancellationToken);
        }

        public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfoAsync(CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient("psybook-api");

            var experienceInfo = await httpClient.GetFromJsonAsync<Dictionary<BookingExperience, ExperienceRecord>>("/Booking/GetExperienceInfo", cancellationToken);

            return experienceInfo ?? [];
        }
    }
}
