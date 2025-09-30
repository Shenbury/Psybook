using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;

namespace Psybook.Services.UI.DataLoaders;

/// <summary>
/// Defines contract for Experience data loading operations in the UI layer.
/// </summary>
public interface IExperienceLoaderService
{
    /// <summary>
    /// Retrieves all experience records for display in the UI.
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
    /// Gets experience records as a dictionary for UI consumption.
    /// </summary>
    /// <returns>A dictionary mapping booking experiences to their records.</returns>
    Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperiencesDictionaryAsync();

    /// <summary>
    /// Creates a new experience record.
    /// </summary>
    /// <param name="experienceRecord">The experience record to create.</param>
    /// <returns>The created experience record.</returns>
    Task<ExperienceRecord> CreateExperienceAsync(ExperienceRecord experienceRecord);

    /// <summary>
    /// Updates an existing experience record.
    /// </summary>
    /// <param name="experienceRecord">The experience record to update.</param>
    /// <returns>The updated experience record.</returns>
    Task<ExperienceRecord> UpdateExperienceAsync(ExperienceRecord experienceRecord);

    /// <summary>
    /// Deletes an experience record.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type to delete.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> DeleteExperienceAsync(BookingExperience bookingExperience);
}