using Microsoft.AspNetCore.Mvc;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Services.API.ExperienceService;

namespace Psybook.API.Controllers;

/// <summary>
/// Controller for managing Experience Records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExperienceController : ControllerBase
{
    private readonly IExperienceService _experienceService;
    private readonly ILogger<ExperienceController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExperienceController"/> class.
    /// </summary>
    /// <param name="experienceService">Service for experience operations.</param>
    /// <param name="logger">Logger for tracking operations.</param>
    public ExperienceController(IExperienceService experienceService, ILogger<ExperienceController> logger)
    {
        _experienceService = experienceService ?? throw new ArgumentNullException(nameof(experienceService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all experience records.
    /// </summary>
    /// <returns>A list of all experience records.</returns>
    [HttpGet]
    public async Task<ActionResult<List<ExperienceRecord>>> GetAllExperiences()
    {
        try
        {
            var experiences = await _experienceService.GetAllExperiencesAsync();
            return Ok(experiences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all experiences");
            return StatusCode(500, "Internal server error occurred while retrieving experiences");
        }
    }

    /// <summary>
    /// Gets a specific experience record by booking experience type.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type.</param>
    /// <returns>The experience record if found.</returns>
    [HttpGet("{bookingExperience}")]
    public async Task<ActionResult<ExperienceRecord>> GetExperience(BookingExperience bookingExperience)
    {
        try
        {
            var experience = await _experienceService.GetExperienceByIdAsync(bookingExperience);
            
            if (experience == null)
            {
                return NotFound($"Experience record for {bookingExperience} not found");
            }

            return Ok(experience);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve experience {BookingExperience}", bookingExperience);
            return StatusCode(500, "Internal server error occurred while retrieving experience");
        }
    }

    /// <summary>
    /// Gets experience records as a dictionary.
    /// </summary>
    /// <returns>A dictionary mapping booking experiences to their records.</returns>
    [HttpGet("dictionary")]
    public async Task<ActionResult<Dictionary<BookingExperience, ExperienceRecord>>> GetExperiencesDictionary()
    {
        try
        {
            var experiencesDictionary = await _experienceService.GetExperiencesDictionaryAsync();
            return Ok(experiencesDictionary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve experiences dictionary");
            return StatusCode(500, "Internal server error occurred while retrieving experiences dictionary");
        }
    }

    /// <summary>
    /// Creates a new experience record.
    /// </summary>
    /// <param name="experienceRecord">The experience record to create.</param>
    /// <returns>The created experience record.</returns>
    [HttpPost]
    public async Task<ActionResult<ExperienceRecord>> CreateExperience([FromBody] ExperienceRecord experienceRecord)
    {
        try
        {
            if (experienceRecord == null)
            {
                return BadRequest("Experience record cannot be null");
            }

            var createdExperience = await _experienceService.CreateExperienceAsync(experienceRecord);
            return CreatedAtAction(
                nameof(GetExperience), 
                new { bookingExperience = createdExperience.BookingExperience }, 
                createdExperience);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid experience record provided");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Experience already exists");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create experience");
            return StatusCode(500, "Internal server error occurred while creating experience");
        }
    }

    /// <summary>
    /// Updates an existing experience record.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type to update.</param>
    /// <param name="experienceRecord">The updated experience record data.</param>
    /// <returns>The updated experience record.</returns>
    [HttpPut("{bookingExperience}")]
    public async Task<ActionResult<ExperienceRecord>> UpdateExperience(
        BookingExperience bookingExperience, 
        [FromBody] ExperienceRecord experienceRecord)
    {
        try
        {
            if (experienceRecord == null)
            {
                return BadRequest("Experience record cannot be null");
            }

            if (bookingExperience != experienceRecord.BookingExperience)
            {
                return BadRequest("Booking experience in URL does not match the one in request body");
            }

            var updatedExperience = await _experienceService.UpdateExperienceAsync(experienceRecord);
            return Ok(updatedExperience);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid experience record provided");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Experience does not exist");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update experience {BookingExperience}", bookingExperience);
            return StatusCode(500, "Internal server error occurred while updating experience");
        }
    }

    /// <summary>
    /// Deletes an experience record.
    /// </summary>
    /// <param name="bookingExperience">The booking experience type to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{bookingExperience}")]
    public async Task<ActionResult> DeleteExperience(BookingExperience bookingExperience)
    {
        try
        {
            var deleted = await _experienceService.DeleteExperienceAsync(bookingExperience);
            
            if (!deleted)
            {
                return NotFound($"Experience record for {bookingExperience} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete experience with existing bookings");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete experience {BookingExperience}", bookingExperience);
            return StatusCode(500, "Internal server error occurred while deleting experience");
        }
    }
}