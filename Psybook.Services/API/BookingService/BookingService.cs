using Psybook.Objects.DbModels;
using Psybook.Repositories.Booking;

namespace Psybook.Services.API.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync()
        {
            return await _bookingRepository.GetCalendarSlotsAsync();
        }
    }
}
