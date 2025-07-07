using Heron.MudCalendar;
using MudBlazor;
using Psybook.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Psybook.Objects.DbModels
{
    public class CalendarSlot : CalendarItem
    {
        public new Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Title { get; set; }
        public required string Location { get; set; }
        public required Color Color { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ContactNumber { get; set; }
        public required string FirstLineAddress { get; set; }
        public required string Postcode { get; set; }
        public required BookingExperience BookingExperience { get; set; }
    }
}