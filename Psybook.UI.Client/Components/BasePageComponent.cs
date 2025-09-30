using Microsoft.AspNetCore.Components;
using MudBlazor;
using Psybook.UI.Client.Services;

namespace Psybook.UI.Client.Components
{
    /// <summary>
    /// Base component for all pages providing common functionality.
    /// Implements Template Method pattern and follows Single Responsibility Principle.
    /// </summary>
    public abstract class BasePageComponent : ComponentBase, IDisposable
    {
        /// <summary>
        /// Gets or sets the snackbar service.
        /// </summary>
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        /// <summary>
        /// Gets or sets the theme service.
        /// </summary>
        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;

        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        [Inject]
        protected INavigationService NavigationService { get; set; } = default!;

        /// <summary>
        /// Gets or sets whether the page is currently loading.
        /// </summary>
        protected bool IsLoading { get; set; } = true;

        /// <summary>
        /// Gets or sets whether an operation is currently in progress.
        /// </summary>
        protected bool IsOperationInProgress { get; set; }

        /// <summary>
        /// Gets or sets the page errors.
        /// </summary>
        protected List<string> PageErrors { get; set; } = new();

        /// <summary>
        /// Gets the page errors (alias for compatibility).
        /// </summary>
        protected List<string> Errors => PageErrors;

        /// <summary>
        /// Gets whether the page has any errors.
        /// </summary>
        protected bool HasErrors => PageErrors.Count > 0;

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                await InitializePageAsync();
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize page", ex);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Template method for page initialization. Override in derived classes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual Task InitializePageAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles errors in a consistent manner across all pages.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="exception">The exception that occurred.</param>
        protected virtual void HandleError(string message, Exception? exception = null)
        {
            var errorMessage = exception?.Message ?? "An unexpected error occurred";
            PageErrors.Add($"{message}: {errorMessage}");
            
            Snackbar.Add(message, Severity.Error);
            
            // Log the error if a logger is available
            LogError(message, exception);
        }

        /// <summary>
        /// Shows a success message to the user.
        /// </summary>
        /// <param name="message">The success message.</param>
        protected void ShowSuccess(string message)
        {
            Snackbar.Add(message, Severity.Success);
        }

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        /// <param name="message">The information message.</param>
        protected void ShowInfo(string message)
        {
            Snackbar.Add(message, Severity.Info);
        }

        /// <summary>
        /// Shows a warning message to the user.
        /// </summary>
        /// <param name="message">The warning message.</param>
        protected void ShowWarning(string message)
        {
            Snackbar.Add(message, Severity.Warning);
        }

        /// <summary>
        /// Clears all page errors.
        /// </summary>
        protected void ClearErrors()
        {
            PageErrors.Clear();
        }

        /// <summary>
        /// Sets the loading state and triggers a UI update.
        /// </summary>
        /// <param name="isLoading">The loading state.</param>
        protected void SetLoadingState(bool isLoading)
        {
            IsLoading = isLoading;
            StateHasChanged();
        }

        /// <summary>
        /// Sets the operation state and triggers a UI update.
        /// </summary>
        /// <param name="isInProgress">The operation state.</param>
        protected void SetOperationState(bool isInProgress)
        {
            IsOperationInProgress = isInProgress;
            StateHasChanged();
        }

        /// <summary>
        /// Executes an operation with loading state management and error handling.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="operationName">The name of the operation for error reporting.</param>
        protected async Task ExecuteOperationAsync(Func<Task> operation, string operationName = "Operation")
        {
            try
            {
                SetOperationState(true);
                await operation();
            }
            catch (Exception ex)
            {
                HandleError($"{operationName} failed", ex);
            }
            finally
            {
                SetOperationState(false);
            }
        }

        /// <summary>
        /// Executes an operation with error handling (alias for compatibility).
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="operationName">The name of the operation for error reporting.</param>
        protected async Task ExecuteWithErrorHandling(Func<Task> operation, string operationName = "Operation")
        {
            await ExecuteOperationAsync(operation, operationName);
        }

        /// <summary>
        /// Logs an error. Override in derived classes to provide specific logging.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exception">The exception.</param>
        protected virtual void LogError(string message, Exception? exception)
        {
            // Base implementation does nothing - override in derived classes with specific logger
        }

        /// <summary>
        /// Disposes of resources used by the component.
        /// </summary>
        public virtual void Dispose()
        {
            // Override in derived classes if needed
        }
    }
}