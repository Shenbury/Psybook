using MudBlazor;
using Psybook.Objects.Enums;

namespace Psybook.Objects.DbModels
{
    public class ExperienceRecord
    {
        public required BookingExperience BookingExperience { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required Color Color { get; set; }
        public required string Location { get; set; }
        public required bool AllDay { get; set; }
        public required TimeSpan Length { get; set; }
    }
}
