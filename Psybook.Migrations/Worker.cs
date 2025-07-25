using Microsoft.EntityFrameworkCore;
using Psybook.Objects.DbModels;
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
        CalendarSlot calendarSlot = new()
        {
            Id = Guid.Parse("0197e4b8-aa07-7c70-b3bc-9c6ba3b3973c"),
            AllDay = false,
            Color = MudBlazor.Color.Primary,
            Start = DateTime.UtcNow.AddHours(1),
            End = DateTime.UtcNow.AddHours(3),
            Location = "Rhinos",
            Text = "VIP Experience: Do A, Do B",
            Title = "VIP Experience",
            BookingExperience = Objects.Enums.BookingExperience.RhinoKeeper,
            ContactNumber = "07572663029",
            FirstLineAddress = "15 Bromsgrove Road",
            FirstName = "Simon",
            LastName = "Henbury",
            Postcode = "B63 3JQ"
        };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var seedRecord = await dbContext.CalendarSlots.SingleOrDefaultAsync(x => x.Title == calendarSlot.Title);

            if(seedRecord is null)
            {
                await dbContext.CalendarSlots.AddAsync(calendarSlot, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        });
    }
}