using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Repositories.Experience;

/// <summary>
/// Defines contract for Experience Record repository operations.
/// </summary>
public interface IExperienceRepository
{
    /// <summary>
    /// Retrieves all experience records.
    /// </summary>
    /// <returns>A list of all experience records.</returns>
    Task<List<ExperienceRecord>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific experience record by booking experience type.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type.</param>
    /// <returns>The experience record if found, null otherwise.</returns>
    Task<ExperienceRecord?> GetByIdAsync(BookingExperience bookingExperience);

    /// <summary>
    /// Creates a new experience record.
    /// </summary>
    /// <param name="experienceRecord">The experience record to create.</param>
    /// <returns>The created experience record.</returns>
    Task<ExperienceRecord> CreateAsync(ExperienceRecord experienceRecord);

    /// <summary>
    /// Updates an existing experience record.
    /// </summary>
    /// <param name="experienceRecord">The experience record to update.</param>
    /// <returns>The updated experience record.</returns>
    Task<ExperienceRecord> UpdateAsync(ExperienceRecord experienceRecord);

    /// <summary>
    /// Deletes an experience record.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type to delete.</param>
    /// <returns>True if the record was deleted, false if it didn't exist.</returns>
    Task<bool> DeleteAsync(BookingExperience bookingExperience);

    /// <summary>
    /// Checks if an experience record exists.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type to check.</param>
    /// <returns>True if the record exists, false otherwise.</returns>
    Task<bool> ExistsAsync(BookingExperience bookingExperience);
}