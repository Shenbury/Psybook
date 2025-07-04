using Psybook.Objects.Enums;

namespace Psybook.Shared.PageModels
{
    public class ExperienceForm
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public string FirstLineAddress { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public BookingExperience BookingExperience { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
