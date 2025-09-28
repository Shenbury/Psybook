using Psybook.Services.ExternalCalendar;

namespace Psybook.Services.UserPreferences
{
    /// <summary>
    /// Represents user preferences for calendar integration
    /// </summary>
    public class UserCalendarPreferences
    {
        public string UserId { get; set; } = string.Empty;
        public bool AutoSyncEnabled { get; set; } = false;
        public List<CalendarProvider> PreferredProviders { get; set; } = new();
        public string DefaultTimeZone { get; set; } = "UTC";
        public bool IncludeCustomerDetails { get; set; } = true;
        public bool SendInvitations { get; set; } = false;
        public bool NotifyOnChanges { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Converts to CalendarIntegrationOptions
        /// </summary>
        public CalendarIntegrationOptions ToIntegrationOptions()
        {
            return new CalendarIntegrationOptions
            {
                AutoSync = AutoSyncEnabled,
                EnabledProviders = PreferredProviders,
                DefaultTimeZone = DefaultTimeZone,
                IncludeCustomerDetails = IncludeCustomerDetails,
                SendInvitations = SendInvitations
            };
        }
    }

    /// <summary>
    /// Service for managing user calendar preferences
    /// </summary>
    public interface IUserCalendarPreferencesService
    {
        /// <summary>
        /// Gets user calendar preferences
        /// </summary>
        Task<UserCalendarPreferences?> GetPreferencesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Saves user calendar preferences
        /// </summary>
        Task SavePreferencesAsync(UserCalendarPreferences preferences, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Deletes user calendar preferences
        /// </summary>
        Task DeletePreferencesAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets default preferences for new users
        /// </summary>
        UserCalendarPreferences GetDefaultPreferences(string userId);
    }

    /// <summary>
    /// In-memory implementation of user calendar preferences service
    /// In production, this would be backed by a database
    /// </summary>
    public class InMemoryUserCalendarPreferencesService : IUserCalendarPreferencesService
    {
        private readonly Dictionary<string, UserCalendarPreferences> _preferences = new();
        private readonly object _lock = new();

        public Task<UserCalendarPreferences?> GetPreferencesAsync(string userId, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _preferences.TryGetValue(userId, out var preferences);
                return Task.FromResult(preferences);
            }
        }

        public Task SavePreferencesAsync(UserCalendarPreferences preferences, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(preferences);
            
            lock (_lock)
            {
                preferences.LastUpdated = DateTime.UtcNow;
                _preferences[preferences.UserId] = preferences;
            }
            
            return Task.CompletedTask;
        }

        public Task DeletePreferencesAsync(string userId, CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _preferences.Remove(userId);
            }
            
            return Task.CompletedTask;
        }

        public UserCalendarPreferences GetDefaultPreferences(string userId)
        {
            return new UserCalendarPreferences
            {
                UserId = userId,
                AutoSyncEnabled = false,
                PreferredProviders = new List<CalendarProvider>(),
                DefaultTimeZone = "UTC",
                IncludeCustomerDetails = true,
                SendInvitations = false,
                NotifyOnChanges = true
            };
        }
    }
}