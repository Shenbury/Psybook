using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MudBlazor;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Shared.Contexts
{
    public class BookingContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<CalendarSlot> CalendarSlots => Set<CalendarSlot>();
        public DbSet<ExperienceRecord> ExperienceRecords => Set<ExperienceRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure CalendarSlot entity
            modelBuilder.Entity<CalendarSlot>(entity =>
            {
                // Color enum conversion
                entity.Property(d => d.Color)
                    .HasConversion(new EnumToStringConverter<Color>());

                // BookingStatus enum conversion
                entity.Property(d => d.Status)
                    .HasConversion(new EnumToStringConverter<BookingStatus>())
                    .HasDefaultValue(BookingStatus.Pending);

                // BookingExperience enum conversion
                entity.Property(d => d.BookingExperience)
                    .HasConversion(new EnumToStringConverter<BookingExperience>());

                // String length constraints
                entity.Property(d => d.CancellationReason)
                    .HasMaxLength(500);

                entity.Property(d => d.ModifiedBy)
                    .HasMaxLength(100);

                entity.Property(d => d.Notes)
                    .HasMaxLength(1000);

                // Indexes for performance
                entity.HasIndex(d => d.Status)
                    .HasDatabaseName("IX_CalendarSlots_Status");

                entity.HasIndex(d => d.Start)
                    .HasDatabaseName("IX_CalendarSlots_Start");

                entity.HasIndex(d => d.BookingExperience)
                    .HasDatabaseName("IX_CalendarSlots_BookingExperience");

                entity.HasIndex(d => d.CreatedAt)
                    .HasDatabaseName("IX_CalendarSlots_CreatedAt");

                // Composite index for common queries
                entity.HasIndex(d => new { d.Status, d.Start })
                    .HasDatabaseName("IX_CalendarSlots_Status_Start");
            });

            // Configure ExperienceRecord entity
            modelBuilder.Entity<ExperienceRecord>(entity =>
            {
                entity.Property(d => d.Color)
                    .HasConversion(new EnumToStringConverter<Color>());

                entity.Property(d => d.BookingExperience)
                    .HasConversion(new EnumToStringConverter<BookingExperience>());
            });
        }
    }
}
