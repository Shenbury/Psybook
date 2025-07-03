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

    }
}
