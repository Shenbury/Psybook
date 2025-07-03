namespace Psybook.Objects.DbModels
{
    public class CalendarSlot
    {
        public required Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Title { get; set; }
        public required string Location { get; set; }
        public required string Color { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime? End { get; set; }
        public required bool AllDay { get; set; }
        public required string Text { get; set; } = string.Empty;
        protected internal bool IsMultiDay => (End == null && Start.TimeOfDay > TimeSpan.FromHours(23)) ||
                                              (End.HasValue && End.Value.Date > Start.Date);
    }
}
