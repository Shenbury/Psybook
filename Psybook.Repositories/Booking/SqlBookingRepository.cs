using Microsoft.EntityFrameworkCore;
using Psybook.Objects.DbModels;
using Psybook.Shared.Contexts;

namespace Psybook.Repositories.Booking
{
    public class SqlBookingRepository: IBookingRepository
    {
        private readonly BookingContext _bookingContext;

        public SqlBookingRepository(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync()
        {
            return await _bookingContext.BookingSlots.AsNoTracking().ToArrayAsync();
        }
    }
}
