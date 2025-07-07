using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MudBlazor;
using Psybook.Objects.DbModels;

namespace Psybook.Shared.Contexts
{
    public class BookingContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<CalendarSlot> CalendarSlots => Set<CalendarSlot>();
        public DbSet<ExperienceRecord> ExperienceRecords => Set<ExperienceRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CalendarSlot>()
                .Property(d => d.Color)
                .HasConversion(new EnumToStringConverter<Color>());

            modelBuilder
                .Entity<ExperienceRecord>()
                .Property(d => d.Color)
                .HasConversion(new EnumToStringConverter<Color>());
        }
    }
}
