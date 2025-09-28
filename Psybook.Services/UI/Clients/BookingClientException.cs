namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// Exception thrown when a booking client operation fails.
    /// </summary>
    public sealed class BookingClientException : Exception
    {
        /// <summary>
        /// Gets the operation that failed.
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Gets the HTTP status code if available.
        /// </summary>
        public int? StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingClientException"/> class.
        /// </summary>
        /// <param name="operation">The operation that failed.</param>
        /// <param name="message">The error message.</param>
        public BookingClientException(string operation, string message) 
            : base(message)
        {
            Operation = operation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingClientException"/> class.
        /// </summary>
        /// <param name="operation">The operation that failed.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BookingClientException(string operation, string message, Exception innerException) 
            : base(message, innerException)
        {
            Operation = operation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingClientException"/> class.
        /// </summary>
        /// <param name="operation">The operation that failed.</param>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="innerException">The inner exception.</param>
        public BookingClientException(string operation, string message, int statusCode, Exception innerException) 
            : base(message, innerException)
        {
            Operation = operation;
            StatusCode = statusCode;
        }
    }
}