namespace Psybook.UI.Client.Services
{
    /// <summary>
    /// Service for managing navigation and routing operations.
    /// Implements Single Responsibility Principle by handling only navigation-related logic.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the home page.
        /// </summary>
        void NavigateToHome();

        /// <summary>
        /// Navigates to the booking page.
        /// </summary>
        void NavigateToBooking();

        /// <summary>
        /// Navigates to a specific URL.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        void NavigateTo(string url);

        /// <summary>
        /// Navigates back in the browser history.
        /// </summary>
        void NavigateBack();
    }
}