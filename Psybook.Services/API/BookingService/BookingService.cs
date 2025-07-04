using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Repositories.Booking;
using Psybook.Shared.Dictionary;

namespace Psybook.Services.API.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ExperienceDictionary _experienceDictionary;

        public BookingService(IBookingRepository bookingRepository, ExperienceDictionary experienceDictionary)
        {
            _bookingRepository = bookingRepository;
            _experienceDictionary = experienceDictionary;
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsAsync()
        {
            return await _bookingRepository.GetCalendarSlotsAsync();
        }

        public async Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot)
        {
            await _bookingRepository.SaveCalendarSlotsAsync(calendarSlot);
        }

        public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
        {
            return await _experienceDictionary.GetExperienceDictionary();
        }
    }
}
