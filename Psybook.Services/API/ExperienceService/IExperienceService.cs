using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.API.ExperienceService;

/// <summary>
/// Defines contract for Experience Record business logic operations.
/// </summary>
public interface IExperienceService
{
    /// <summary>
    /// Retrieves all experience records.
    /// </summary>
    /// <returns>A list of all experience records.</returns>
    Task<List<ExperienceRecord>> GetAllExperiencesAsync();

    /// <summary>
    /// Retrieves a specific experience record by booking experience type.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type.</param>
    /// <returns>The experience record if found, null otherwise.</returns>
    Task<ExperienceRecord?> GetExperienceByIdAsync(BookingExperience bookingExperience);

    /// <summary>
    /// Creates a new experience record with validation.
    /// </summary>
    /// <param name="experienceRecord">The experience record to create.</param>
    /// <returns>The created experience record.</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when experience already exists.</exception>
    Task<ExperienceRecord> CreateExperienceAsync(ExperienceRecord experienceRecord);

    /// <summary>
    /// Updates an existing experience record with validation.
    /// </summary>
    /// <param name="experienceRecord">The experience record to update.</param>
    /// <returns>The updated experience record.</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when experience doesn't exist.</exception>
    Task<ExperienceRecord> UpdateExperienceAsync(ExperienceRecord experienceRecord);

    /// <summary>
    /// Deletes an experience record.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type to delete.</param>
    /// <returns>True if the record was deleted, false if it didn't exist.</returns>
    /// <exception cref="InvalidOperationException">Thrown when experience cannot be deleted due to existing bookings.</exception>
    Task<bool> DeleteExperienceAsync(BookingExperience bookingExperience);

    /// <summary>
    /// Gets experience records as a dictionary for UI consumption.
    /// </summary>
    /// <returns>A dictionary mapping booking experiences to their records.</returns>
    Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperiencesDictionaryAsync();
}