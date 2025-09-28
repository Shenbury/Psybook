using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Psybook.UI.Client.Services
{
    /// <summary>
    /// Implementation of navigation service for managing routing operations.
    /// </summary>
    public sealed class NavigationService : INavigationService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="navigationManager">The navigation manager.</param>
        /// <param name="jsRuntime">The JavaScript runtime.</param>
        public NavigationService(NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        /// <inheritdoc />
        public void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }

        /// <inheritdoc />
        public void NavigateToBooking()
        {
            _navigationManager.NavigateTo("/book");
        }

        /// <inheritdoc />
        public void NavigateTo(string url)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url);
            _navigationManager.NavigateTo(url);
        }

        /// <inheritdoc />
        public void NavigateBack()
        {
            _ = _jsRuntime.InvokeVoidAsync("history.back");
        }
    }
}