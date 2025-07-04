using Microsoft.EntityFrameworkCore;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Shared.Contexts;

namespace Psybook.Objects.Dictionary
{
    public class ExperienceDictionary(BookingContext bookingContext)
    {
        private static readonly Dictionary<BookingExperience, ExperienceRecord> _bookingExperienceRecords = [];

        public async Task<IDictionary<BookingExperience, ExperienceRecord>> GetExperienceDictionary()
        {
            if (_bookingExperienceRecords.Any())
            {
                return _bookingExperienceRecords;
            }

            List<ExperienceRecord> experiences = await bookingContext.ExperienceRecords.ToListAsync();
            foreach (ExperienceRecord? experience in experiences)
            {
                _bookingExperienceRecords.Add(experience.BookingExperience, experience);
            }

            return _bookingExperienceRecords;
        }
    }
}
