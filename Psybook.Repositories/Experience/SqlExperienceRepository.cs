using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Shared.Contexts;

namespace Psybook.Repositories.Experience;

/// <summary>
/// SQL Server implementation of the Experience Repository.
/// </summary>
public class SqlExperienceRepository : IExperienceRepository
{
    private readonly BookingContext _context;
    private readonly ILogger<SqlExperienceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlExperienceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">Logger for tracking operations.</param>
    public SqlExperienceRepository(BookingContext context, ILogger<SqlExperienceRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<ExperienceRecord>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all experience records");
            var records = await _context.ExperienceRecords
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} experience records", records.Count);
            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve experience records");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord?> GetByIdAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Retrieving experience record for {BookingExperience}", bookingExperience);
            var record = await _context.ExperienceRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.BookingExperience == bookingExperience);

            if (record == null)
            {
                _logger.LogWarning("Experience record not found for {BookingExperience}", bookingExperience);
            }

            return record;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve experience record for {BookingExperience}", bookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> CreateAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);

            _logger.LogInformation("Creating experience record for {BookingExperience}", experienceRecord.BookingExperience);
            
            _context.ExperienceRecords.Add(experienceRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created experience record for {BookingExperience}", experienceRecord.BookingExperience);
            return experienceRecord;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create experience record for {BookingExperience}", experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ExperienceRecord> UpdateAsync(ExperienceRecord experienceRecord)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(experienceRecord);

            _logger.LogInformation("Updating experience record for {BookingExperience}", experienceRecord.BookingExperience);
            
            _context.ExperienceRecords.Update(experienceRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated experience record for {BookingExperience}", experienceRecord.BookingExperience);
            return experienceRecord;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update experience record for {BookingExperience}", experienceRecord?.BookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(BookingExperience bookingExperience)
    {
        try
        {
            _logger.LogInformation("Deleting experience record for {BookingExperience}", bookingExperience);
            
            var record = await _context.ExperienceRecords
                .FirstOrDefaultAsync(e => e.BookingExperience == bookingExperience);

            if (record == null)
            {
                _logger.LogWarning("Experience record not found for deletion: {BookingExperience}", bookingExperience);
                return false;
            }

            _context.ExperienceRecords.Remove(record);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted experience record for {BookingExperience}", bookingExperience);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete experience record for {BookingExperience}", bookingExperience);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(BookingExperience bookingExperience)
    {
        try
        {
            var exists = await _context.ExperienceRecords
                .AnyAsync(e => e.BookingExperience == bookingExperience);

            _logger.LogDebug("Experience record exists check for {BookingExperience}: {Exists}", bookingExperience, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if experience record exists for {BookingExperience}", bookingExperience);
            throw;
        }
    }
}