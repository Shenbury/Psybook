# External Calendar Integration

This documentation describes the comprehensive external calendar integration system implemented for the WMSP VIP Booking System.

## Overview

The external calendar integration allows users to sync their VIP experience bookings with popular calendar platforms including Google Calendar, Outlook, Apple Calendar, and any application supporting iCalendar (.ics) files.

## Features

### ??? **Supported Calendar Providers**
- **Google Calendar** - URL-based quick add with future API integration support
- **Outlook Calendar** - Direct integration with Outlook Web and desktop
- **Apple Calendar** - iCalendar file support for seamless iOS/macOS integration
- **iCalendar (.ics)** - Universal calendar file format for any compatible application

### ?? **Integration Methods**
1. **Quick Add URLs** - One-click calendar addition via provider-specific URLs
2. **iCalendar Downloads** - Downloadable .ics files for manual import
3. **Auto-Sync** - Automated synchronization based on user preferences
4. **Bulk Operations** - Sync multiple bookings simultaneously

### ??? **User Preferences**
- Enable/disable automatic synchronization
- Select preferred calendar providers
- Include/exclude customer details in calendar events
- Configure default timezone settings
- Set up notification preferences

## Architecture

### Core Components

#### 1. **External Calendar Models** (`ExternalCalendarModels.cs`)
```csharp
// Main event model for calendar integration
public class ExternalCalendarEvent

// Integration result tracking
public class CalendarIntegrationResult

// Configuration options
public class CalendarIntegrationOptions
```

#### 2. **Calendar Service** (`IExternalCalendarService.cs`, `ExternalCalendarService.cs`)
```csharp
// Primary service interface for calendar operations
public interface IExternalCalendarService

// Implementation with URL generation and iCal creation
public class ExternalCalendarService
```

#### 3. **UI Components**
- `CalendarIntegrationComponent.razor` - User-facing integration interface
- `BookingDetailsDialog.razor` - Enhanced with calendar integration
- `CalendarTest.razor` - Comprehensive testing interface

#### 4. **API Integration** (`CalendarIntegrationController.cs`)
```csharp
// RESTful API endpoints for calendar operations
[Route("api/[controller]")]
public class CalendarIntegrationController
```

## Usage Examples

### Basic Integration in Blazor Components

```razor
@using Psybook.Services.ExternalCalendar

<CalendarIntegrationComponent Booking="@myBooking" 
                            ShowAutoSyncOption="true"
                            OnAutoSyncConfigured="HandleAutoSync" />
```

### Programmatic Calendar Operations

```csharp
// Generate calendar URLs
var calendarEvent = ExternalCalendarEvent.FromCalendarSlot(booking);
var urls = calendarService.GenerateCalendarUrls(calendarEvent);

// Create iCalendar file
var icalData = await calendarService.GenerateICalendarFileAsync(calendarEvent);

// Sync with multiple providers
var options = new CalendarIntegrationOptions
{
    EnabledProviders = { CalendarProvider.GoogleCalendar, CalendarProvider.OutlookCalendar }
};
var results = await calendarService.SyncBookingAsync(booking, options);
```

## API Endpoints

### GET `/api/calendarintegration/booking/{bookingId}/icalendar`
Downloads an iCalendar (.ics) file for a specific booking.

**Response**: Binary .ics file

### GET `/api/calendarintegration/booking/{bookingId}/urls`
Returns calendar URLs for quick-add functionality.

**Response**:
```json
{
  "bookingId": "guid",
  "urls": {
    "GoogleCalendar": "https://calendar.google.com/calendar/render?...",
    "OutlookCalendar": "https://outlook.live.com/calendar/...",
    "AppleCalendar": "data:text/calendar;base64,..."
  }
}
```

### POST `/api/calendarintegration/booking/{bookingId}/sync`
Synchronizes a booking with external calendars.

**Request Body**:
```json
{
  "autoSync": true,
  "providers": ["GoogleCalendar", "OutlookCalendar"],
  "includeCustomerDetails": true,
  "sendInvitations": false
}
```

## Configuration

### Dependency Injection Setup

#### Client-Side (Blazor WebAssembly)
```csharp
// Program.cs
builder.Services.AddScoped<IExternalCalendarService, ExternalCalendarService>();
```

#### Server-Side (API)
```csharp
// Program.cs
builder.Services.AddScoped<IExternalCalendarService, ExternalCalendarService>();
builder.Services.AddScoped<GoogleCalendarApiService>();
builder.Services.AddHttpClient(); // Required for HTTP operations
```

### JavaScript Integration

Include the calendar integration JavaScript file in your layout:

```html
<script src="js/calendar-integration.js"></script>
```

## Advanced Features

### Google Calendar API Integration

For full Google Calendar API integration (create, update, delete events):

1. Set up Google Cloud Console project
2. Enable Calendar API
3. Configure OAuth 2.0 credentials
4. Implement token management

```csharp
// Example API integration
var googleService = serviceProvider.GetService<GoogleCalendarApiService>();
var result = await googleService.CreateEventAsync(accessToken, calendarId, googleEvent);
```

### User Preferences Management

Store and retrieve user calendar preferences:

```csharp
// Save user preferences
var preferences = new UserCalendarPreferences
{
    UserId = currentUserId,
    AutoSyncEnabled = true,
    PreferredProviders = { CalendarProvider.GoogleCalendar }
};
await preferencesService.SavePreferencesAsync(preferences);
```

## Testing

### Comprehensive Test Page

Navigate to `/calendar-test` to access the testing interface:

1. **Create Test Bookings** - Generate sample booking data
2. **Test Integration** - Verify calendar URL generation
3. **iCal Validation** - Download and validate iCalendar files
4. **Bulk Sync Testing** - Test multiple provider synchronization

### Unit Testing

```csharp
[Test]
public async Task GenerateCalendarUrls_ReturnsValidUrls()
{
    // Arrange
    var service = new ExternalCalendarService(logger, httpClientFactory);
    var calendarEvent = new ExternalCalendarEvent { /* test data */ };
    
    // Act
    var urls = service.GenerateCalendarUrls(calendarEvent);
    
    // Assert
    Assert.IsTrue(urls.ContainsKey(CalendarProvider.GoogleCalendar));
    Assert.IsTrue(urls[CalendarProvider.GoogleCalendar].StartsWith("https://"));
}
```

## Security Considerations

1. **Data Privacy** - User controls what information is included in calendar events
2. **Access Control** - Calendar integration respects existing authentication
3. **URL Safety** - All generated URLs are properly encoded and validated
4. **Token Management** - OAuth tokens are handled securely (when using APIs)

## Browser Compatibility

- **Modern Browsers**: Full support for all features
- **File Downloads**: Supported in Chrome, Firefox, Safari, Edge
- **Clipboard API**: Available in HTTPS contexts
- **Calendar URLs**: Universal support across all platforms

## Troubleshooting

### Common Issues

1. **Calendar not opening**: Check popup blockers and browser settings
2. **iCal download fails**: Verify file permissions and browser download settings
3. **Invalid dates**: Ensure timezone and date format compatibility
4. **Missing events**: Check calendar provider import settings

### Debug Mode

Enable detailed logging:

```csharp
services.Configure<LoggerFilterOptions>(options =>
{
    options.AddFilter("Psybook.Services.ExternalCalendar", LogLevel.Debug);
});
```

## Future Enhancements

1. **Real-time Sync** - Bidirectional synchronization with calendar providers
2. **Recurring Events** - Support for recurring VIP experiences
3. **Conflict Detection** - Identify scheduling conflicts across calendars
4. **Mobile Apps** - Native mobile calendar integration
5. **Teams Integration** - Microsoft Teams calendar support
6. **Webhook Support** - Real-time updates from calendar providers

## Contributing

When contributing to the calendar integration system:

1. Follow existing patterns for new calendar providers
2. Maintain backward compatibility with existing integrations
3. Add comprehensive tests for new features
4. Update this documentation for any new functionality

## License

This calendar integration system is part of the WMSP VIP Booking System and follows the same licensing terms.