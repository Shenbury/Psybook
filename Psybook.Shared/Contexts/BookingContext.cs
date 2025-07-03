using Microsoft.EntityFrameworkCore;
using Psybook.Objects.DbModels;

namespace Psybook.Shared.Contexts
{
    public class BookingContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<CalendarSlot> BookingSlots => Set<CalendarSlot>();
    }
}
