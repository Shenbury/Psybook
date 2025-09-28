namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// Configuration options for the BookingClient.
    /// </summary>
    public sealed class BookingClientOptions
    {
        /// <summary>
        /// The timeout for HTTP requests. Default is 30 seconds.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The maximum number of items to retrieve in a single request. Default is 1000.
        /// </summary>
        public int MaxItemsPerRequest { get; set; } = 1000;

        /// <summary>
        /// Whether to validate response content. Default is true.
        /// </summary>
        public bool ValidateResponses { get; set; } = true;
    }
}