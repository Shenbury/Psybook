using Heron.MudCalendar;

namespace Psybook.Objects.DbModels
{
    public class CalendarSlot : CalendarItem
    {
        public required Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Title { get; set; }
        public required string Location { get; set; }
        public required string Color { get; set; }
    }
}
