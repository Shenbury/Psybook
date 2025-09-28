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

        public async Task<CalendarSlot?> GetCalendarSlotByIdAsync(Guid id)
        {
            return await _bookingRepository.GetCalendarSlotByIdAsync(id);
        }

        public async Task<IEnumerable<CalendarSlot>> GetCalendarSlotsByStatusAsync(BookingStatus status)
        {
            return await _bookingRepository.GetCalendarSlotsByStatusAsync(status);
        }

        public async Task SaveCalendarSlotsAsync(CalendarSlot calendarSlot)
        {
            await _bookingRepository.SaveCalendarSlotsAsync(calendarSlot);
        }

        public async Task UpdateCalendarSlotAsync(CalendarSlot calendarSlot)
        {
            await _bookingRepository.UpdateCalendarSlotAsync(calendarSlot);
        }

        public async Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus newStatus, string? reason = null, string? modifiedBy = null)
        {
            await _bookingRepository.UpdateBookingStatusAsync(bookingId, newStatus, reason, modifiedBy);
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId, string reason, string? modifiedBy = null)
        {
            var booking = await _bookingRepository.GetCalendarSlotByIdAsync(bookingId);
            if (booking == null || !booking.IsCancellable)
            {
                return false;
            }

            await _bookingRepository.UpdateBookingStatusAsync(bookingId, BookingStatus.Cancelled, reason, modifiedBy);
            return true;
        }

        public async Task<bool> ConfirmBookingAsync(Guid bookingId, string? modifiedBy = null)
        {
            var booking = await _bookingRepository.GetCalendarSlotByIdAsync(bookingId);
            if (booking == null || booking.Status != BookingStatus.Pending)
            {
                return false;
            }

            await _bookingRepository.UpdateBookingStatusAsync(bookingId, BookingStatus.Confirmed, null, modifiedBy);
            return true;
        }

        public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperienceInfo()
        {
            return await _experienceDictionary.GetExperienceDictionary();
        }
    }
}
