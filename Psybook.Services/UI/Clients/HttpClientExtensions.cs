using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// Extension methods for common HTTP client operations with consistent error handling.
    /// </summary>
    internal static class HttpClientExtensions
    {
        /// <summary>
        /// Executes an HTTP operation with consistent error handling and logging.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="operation">The HTTP operation to execute.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="operationName">The name of the operation for logging.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="callerName">The name of the calling method (automatically populated).</param>
        /// <returns>The result of the operation.</returns>
        public static async Task<T> ExecuteWithErrorHandlingAsync<T>(
            Func<Task<T>> operation,
            ILogger logger,
            string operationName,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string callerName = "")
        {
            using var activity = Activity.Current?.Source.StartActivity($"BookingClient.{callerName}");
            activity?.SetTag("operation", operationName);

            logger.LogDebug("Starting {Operation}", operationName);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await operation();
                
                stopwatch.Stop();
                activity?.SetTag("success", true);
                activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);
                
                logger.LogInformation("Successfully completed {Operation} in {Duration}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                activity?.SetTag("success", false);
                activity?.SetTag("error", ex.Message);
                activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);
                
                logger.LogError(ex, "HTTP request failed for {Operation} after {Duration}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                stopwatch.Stop();
                activity?.SetTag("success", false);
                activity?.SetTag("cancelled", true);
                activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);
                
                logger.LogWarning("Request for {Operation} was cancelled after {Duration}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                activity?.SetTag("success", false);
                activity?.SetTag("error", ex.Message);
                activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);
                
                logger.LogError(ex, "Unexpected error occurred during {Operation} after {Duration}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Validates that a response contains expected data.
        /// </summary>
        /// <typeparam name="T">The type of the response data.</typeparam>
        /// <param name="data">The response data to validate.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="operationName">The name of the operation for logging.</param>
        public static void ValidateResponse<T>(T? data, ILogger logger, string operationName)
        {
            if (data is null)
            {
                logger.LogWarning("Received null response for {Operation}", operationName);
            }
        }
    }
}