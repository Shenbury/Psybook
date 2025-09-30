using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.UI.Clients;

namespace Psybook.Services.UI.DataLoaders;

/// <summary>
/// Service for loading experience data from the API for UI components.
/// Handles caching, error handling, and data transformation for the UI layer.
/// </summary>
public sealed class ExperienceDataLoaderService : IExperienceLoaderService
{
    private readonly IExperienceClient _experienceClient;
    private readonly ILogger<ExperienceDataLoaderService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExperienceDataLoaderService"/> class.
    /// </summary>
    /// <param name="experienceClient">The experience client for API operations.</param>
    /// <param name="logger">Logger for tracking operations and errors.</param>
    public ExperienceDataLoaderService(IExperienceClient experienceClient, ILogger<ExperienceDataLoaderService> logger)
    {
        _experienceClient = experienceClient ?? throw new ArgumentNullException(nameof(experienceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<ExperienceRecord>> GetAllExperiencesAsync()
    {
        try
        {
            _logger.LogInformation("Loading all experiences for UI display");
            var experiences = await _experienceClient.GetAllExperiencesAsync();
            
            _logger.LogInformation("Successfully loaded {ExperienceCount} experiences", experiences.Count);
            return experiences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load experiences for UI display");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord?> GetExperienceByIdAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Loading experience {BookingExperience} for UI display", bookingExperience);
            var experience = await _experienceClient.GetExperienceByIdAsync(bookingExperience);
            
            if (experience != null)
            {
                _logger.LogInformation("Successfully loaded experience {BookingExperience}", bookingExperience);
            }
            else
            {
                _logger.LogWarning("Experience {BookingExperience} not found", bookingExperience);
            }
            
            return experience;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load experience {BookingExperience} for UI display", bookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperiencesDictionaryAsync()
    {
        try
        {
            _logger.LogInformation("Loading experiences dictionary for UI display");
            var experiencesDictionary = await _experienceClient.GetExperiencesDictionaryAsync();
            
            _logger.LogInformation("Successfully loaded experiences dictionary with {Count} entries", 
                experiencesDictionary.Count);
            return experiencesDictionary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load experiences dictionary for UI display");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> CreateExperienceAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);
            
            _logger.LogInformation("Creating experience {BookingExperience} from UI", experienceRecord.BookingExperience);
            var createdExperience = await _experienceClient.CreateExperienceAsync(experienceRecord);
            
            _logger.LogInformation("Successfully created experience {BookingExperience}", 
                createdExperience.BookingExperience);
            return createdExperience;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create experience {BookingExperience} from UI", 
                experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> UpdateExperienceAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);
            
            _logger.LogInformation("Updating experience {BookingExperience} from UI", experienceRecord.BookingExperience);
            var updatedExperience = await _experienceClient.UpdateExperienceAsync(experienceRecord);
            
            _logger.LogInformation("Successfully updated experience {BookingExperience}", 
                updatedExperience.BookingExperience);
            return updatedExperience;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update experience {BookingExperience} from UI", 
                experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteExperienceAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Deleting experience {BookingExperience} from UI", bookingExperience);
            var deleted = await _experienceClient.DeleteExperienceAsync(bookingExperience);
            
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted experience {BookingExperience}", bookingExperience);
            }
            else
            {
                _logger.LogWarning("Experience {BookingExperience} not found for deletion", bookingExperience);
            }
            
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete experience {BookingExperience} from UI", bookingExperience);
            throw;
        }
    }
}