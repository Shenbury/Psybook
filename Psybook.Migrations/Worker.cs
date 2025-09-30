using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Psybook.Objects.DbModels;
using Psybook.Objects.Enums;
using Psybook.Shared.Contexts;
using System.Diagnostics;

namespace Psybook.Migrations;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(BookingContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(BookingContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            // Seed Experience Records
            await SeedExperienceRecords(dbContext, cancellationToken);
            
            // Seed Calendar Slots
            await SeedCalendarSlots(dbContext, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }

    private static async Task SeedExperienceRecords(BookingContext dbContext, CancellationToken cancellationToken)
    {
        // Check if experience records already exist
        if (await dbContext.ExperienceRecords.AnyAsync(cancellationToken))
        {
            return; // Already seeded
        }

        var experienceRecords = new List<ExperienceRecord>
        {
            new()
            {
                BookingExperience = BookingExperience.RhinoKeeper,
                Title = "Rhino Keeper Experience",
                Description = "Get up close with our magnificent rhinos! Help prepare their meals, learn about their behavior, and participate in their daily care routine.",
                Color = Color.Primary,
                Location = "Rhino Habitat",
                AllDay = false,
                Length = TimeSpan.FromHours(2)
            },
            new()
            {
                BookingExperience = BookingExperience.ElephantKeeper,
                Title = "Elephant Keeper Experience",
                Description = "Join our elephant keepers for an unforgettable day caring for these gentle giants. Feed, bathe, and learn about elephant conservation.",
                Color = Color.Secondary,
                Location = "Elephant Paddock",
                AllDay = false,
                Length = TimeSpan.FromHours(3)
            },
            new()
            {
                BookingExperience = BookingExperience.BigCatKeeper,
                Title = "Big Cat Keeper Experience",
                Description = "Experience the thrill of working with our big cats including lions, tigers, and leopards. Observe feeding time and learn about their hunting behaviors.",
                Color = Color.Warning,
                Location = "Big Cat Territory",
                AllDay = false,
                Length = TimeSpan.FromHours(2.5)
            },
            new()
            {
                BookingExperience = BookingExperience.PrimateKeeper,
                Title = "Primate Keeper Experience",
                Description = "Discover the fascinating world of primates! Help with enrichment activities and learn about their complex social structures.",
                Color = Color.Success,
                Location = "Primate House",
                AllDay = false,
                Length = TimeSpan.FromHours(2)
            },
            new()
            {
                BookingExperience = BookingExperience.VIPTour,
                Title = "VIP Behind-the-Scenes Tour",
                Description = "An exclusive all-day experience including multiple animal encounters, private guided tours, and lunch with our conservation team.",
                Color = Color.Tertiary,
                Location = "Various Locations",
                AllDay = true,
                Length = TimeSpan.FromHours(8)
            },
            new()
            {
                BookingExperience = BookingExperience.GiraffeKeeper,
                Title = "Giraffe Keeper Experience",
                Description = "Join our giraffe keepers and learn about these amazing gentle giants. Help with feeding and enrichment activities.",
                Color = Color.Info,
                Location = "Giraffe House",
                AllDay = false,
                Length = TimeSpan.FromHours(2)
            }
        };

        await dbContext.ExperienceRecords.AddRangeAsync(experienceRecords, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedCalendarSlots(BookingContext dbContext, CancellationToken cancellationToken)
    {
        CalendarSlot calendarSlot = new()
        {
            Id = Guid.Parse("0197e4b8-aa07-7c70-b3bc-9c6ba3b3973c"),
            AllDay = false,
            Color = Color.Primary,
            Start = DateTime.UtcNow.AddHours(1),
            End = DateTime.UtcNow.AddHours(3),
            Location = "Rhinos",
            Text = "VIP Experience: Do A, Do B",
            Title = "VIP Experience",
            BookingExperience = BookingExperience.RhinoKeeper,
            ContactNumber = "07572663029",
            FirstLineAddress = "15 Bromsgrove Road",
            FirstName = "Simon",
            LastName = "Henbury",
            Postcode = "B63 3JQ"
        };

        var seedRecord = await dbContext.CalendarSlots.SingleOrDefaultAsync(x => x.Title == calendarSlot.Title, cancellationToken);

        if (seedRecord is null)
        {
            await dbContext.CalendarSlots.AddAsync(calendarSlot, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}