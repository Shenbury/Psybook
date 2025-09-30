using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using System.Net.Http.Json;

namespace Psybook.Services.UI.Clients;

/// <summary>
/// Client for Experience API operations.
/// </summary>
public sealed class ExperienceClient : IExperienceClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExperienceClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExperienceClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
    /// <param name="logger">Logger for tracking operations.</param>
    public ExperienceClient(IHttpClientFactory httpClientFactory, ILogger<ExperienceClient> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<ExperienceRecord>> GetAllExperiencesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all experiences from API");
            
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.GetFromJsonAsync<List<ExperienceRecord>>(
                ExperienceClientConstants.BaseRoute);

            var experiences = response ?? new List<ExperienceRecord>();
            _logger.LogInformation("Successfully fetched {Count} experiences from API", experiences.Count);
            
            return experiences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch experiences from API");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord?> GetExperienceByIdAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Fetching experience {BookingExperience} from API", bookingExperience);
            
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.GetFromJsonAsync<ExperienceRecord>(
                $"{ExperienceClientConstants.BaseRoute}/{bookingExperience}");

            if (response != null)
            {
                _logger.LogInformation("Successfully fetched experience {BookingExperience} from API", bookingExperience);
            }
            else
            {
                _logger.LogWarning("Experience {BookingExperience} not found in API", bookingExperience);
            }
            
            return response;
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            _logger.LogWarning("Experience {BookingExperience} not found in API", bookingExperience);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch experience {BookingExperience} from API", bookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<BookingExperience, ExperienceRecord>> GetExperiencesDictionaryAsync()
    {
        try
        {
            _logger.LogInformation("Fetching experiences dictionary from API");
            
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.GetFromJsonAsync<Dictionary<BookingExperience, ExperienceRecord>>(
                ExperienceClientConstants.DictionaryRoute);

            var experiencesDictionary = response ?? new Dictionary<BookingExperience, ExperienceRecord>();
            _logger.LogInformation("Successfully fetched experiences dictionary with {Count} entries from API", 
                experiencesDictionary.Count);
            
            return experiencesDictionary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch experiences dictionary from API");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> CreateExperienceAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);
            
            _logger.LogInformation("Creating experience {BookingExperience} via API", experienceRecord.BookingExperience);
            
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.PostAsJsonAsync(
                ExperienceClientConstants.BaseRoute, experienceRecord);

            response.EnsureSuccessStatusCode();

            var createdExperience = await response.Content.ReadFromJsonAsync<ExperienceRecord>();
            if (createdExperience == null)
            {
                throw new InvalidOperationException("Failed to deserialize created experience from API response");
            }

            _logger.LogInformation("Successfully created experience {BookingExperience} via API", 
                createdExperience.BookingExperience);
            
            return createdExperience;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create experience {BookingExperience} via API", 
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
            
            _logger.LogInformation("Updating experience {BookingExperience} via API", experienceRecord.BookingExperience);
            
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.PutAsJsonAsync(
                $"{ExperienceClientConstants.BaseRoute}/{experienceRecord.BookingExperience}", 
                experienceRecord);

            response.EnsureSuccessStatusCode();

            var updatedExperience = await response.Content.ReadFromJsonAsync<ExperienceRecord>();
            if (updatedExperience == null)
            {
                throw new InvalidOperationException("Failed to deserialize updated experience from API response");
            }

            _logger.LogInformation("Successfully updated experience {BookingExperience} via API", 
                updatedExperience.BookingExperience);
            
            return updatedExperience;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update experience {BookingExperience} via API", 
                experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteExperienceAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Deleting experience {BookingExperience} via API", bookingExperience);
            
            using var httpClient = CreateConfiguredHttpClient();
            var response = await httpClient.DeleteAsync(
                $"{ExperienceClientConstants.BaseRoute}/{bookingExperience}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted experience {BookingExperience} via API", bookingExperience);
                return true;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Experience {BookingExperience} not found for deletion via API", bookingExperience);
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete experience {BookingExperience} via API", bookingExperience);
            throw;
        }
    }

    /// <summary>
    /// Creates and configures an HTTP client for experience API operations.
    /// </summary>
    /// <returns>A configured HTTP client.</returns>
    private HttpClient CreateConfiguredHttpClient()
    {
        return _httpClientFactory.CreateClient(BookingClientConstants.HttpClientName);
    }
}