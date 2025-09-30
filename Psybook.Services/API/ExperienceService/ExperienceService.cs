using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Repositories.Booking;
using Psybook.Repositories.Experience;

namespace Psybook.Services.API.ExperienceService;

/// <summary>
/// Service for managing Experience Record business logic.
/// </summary>
public class ExperienceService : IExperienceService
{
    private readonly IExperienceRepository _experienceRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<ExperienceService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExperienceService"/> class.
    /// </summary>
    /// <param name="experienceRepository">Repository for experience operations.</param>
    /// <param name="bookingRepository">Repository for booking operations (for validation).</param>
    /// <param name="logger">Logger for tracking operations.</param>
    public ExperienceService(
        IExperienceRepository experienceRepository,
        IBookingRepository bookingRepository,
        ILogger<ExperienceService> logger)
    {
        _experienceRepository = experienceRepository ?? throw new ArgumentNullException(nameof(experienceRepository));
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<ExperienceRecord>> GetAllExperiencesAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all experiences");
            return await _experienceRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all experiences");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord?> GetExperienceByIdAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Retrieving experience: {BookingExperience}", bookingExperience);
            return await _experienceRepository.GetByIdAsync(bookingExperience);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve experience: {BookingExperience}", bookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> CreateExperienceAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);
            
            _logger.LogInformation("Creating experience: {BookingExperience}", experienceRecord.BookingExperience);

            // Validate the experience record
            ValidateExperienceRecord(experienceRecord);

            // Check if experience already exists
            var exists = await _experienceRepository.ExistsAsync(experienceRecord.BookingExperience);
            if (exists)
            {
                throw new InvalidOperationException($"Experience record for {experienceRecord.BookingExperience} already exists");
            }

            return await _experienceRepository.CreateAsync(experienceRecord);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to create experience: {BookingExperience}", experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> UpdateExperienceAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);
            
            _logger.LogInformation("Updating experience: {BookingExperience}", experienceRecord.BookingExperience);

            // Validate the experience record
            ValidateExperienceRecord(experienceRecord);

            // Check if experience exists
            var exists = await _experienceRepository.ExistsAsync(experienceRecord.BookingExperience);
            if (!exists)
            {
                throw new InvalidOperationException($"Experience record for {experienceRecord.BookingExperience} does not exist");
            }

            return await _experienceRepository.UpdateAsync(experienceRecord);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to update experience: {BookingExperience}", experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteExperienceAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Deleting experience: {BookingExperience}", bookingExperience);

            // Check if there are any bookings for this experience
            var bookings = await _bookingRepository.GetCalendarSlotsByExperienceAsync(bookingExperience);
            if (bookings.Any())
            {
                throw new InvalidOperationException($"Cannot delete experience {bookingExperience} because there are existing bookings for it");
            }

            return await _experienceRepository.DeleteAsync(bookingExperience);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to delete experience: {BookingExperience}", bookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperiencesDictionaryAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving experiences dictionary");
            var experiences = await _experienceRepository.GetAllAsync();
            return experiences.ToDictionary(e => e.BookingExperience, e => e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve experiences dictionary");
            throw;
        }
    }

    private static void ValidateExperienceRecord(ExperienceRecord experienceRecord)
    {
        if (string.IsNullOrWhiteSpace(experienceRecord.Title))
        {
            throw new ArgumentException("Experience title cannot be empty", nameof(experienceRecord));
        }

        if (string.IsNullOrWhiteSpace(experienceRecord.Description))
        {
            throw new ArgumentException("Experience description cannot be empty", nameof(experienceRecord));
        }

        if (string.IsNullOrWhiteSpace(experienceRecord.Location))
        {
            throw new ArgumentException("Experience location cannot be empty", nameof(experienceRecord));
        }

        if (experienceRecord.Length <= TimeSpan.Zero)
        {
            throw new ArgumentException("Experience length must be greater than zero", nameof(experienceRecord));
        }

        if (experienceRecord.BookingExperience == BookingExperience.None)
        {
            throw new ArgumentException("BookingExperience cannot be None", nameof(experienceRecord));
        }
    }
}