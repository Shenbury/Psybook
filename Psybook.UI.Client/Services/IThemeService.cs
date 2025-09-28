using MudBlazor;

namespace Psybook.UI.Client.Services
{
    /// <summary>
    /// Service for managing UI theme and appearance settings.
    /// Implements Single Responsibility Principle by handling only theme-related operations.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the current theme configuration.
        /// </summary>
        MudTheme CurrentTheme { get; }

        /// <summary>
        /// Gets or sets whether dark mode is enabled.
        /// </summary>
        bool IsDarkMode { get; set; }

        /// <summary>
        /// Toggles between light and dark mode.
        /// </summary>
        void ToggleDarkMode();

        /// <summary>
        /// Event raised when the theme changes.
        /// </summary>
        event Action? ThemeChanged;
    }
}