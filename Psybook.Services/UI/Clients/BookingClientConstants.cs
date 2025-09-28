namespace Psybook.Services.UI.Clients
{
    /// <summary>
    /// Contains constants used by the BookingClient.
    /// </summary>
    internal static class BookingClientConstants
    {
        /// <summary>
        /// The name of the HTTP client used for API calls.
        /// </summary>
        public const string HttpClientName = "psybook-api";

        /// <summary>
        /// API endpoint routes for booking operations.
        /// </summary>
        public static class ApiRoutes
        {
            /// <summary>
            /// Endpoint to retrieve calendar slots.
            /// </summary>
            public const string GetCalendarSlots = "/Booking/GetCalendarSlots";

            /// <summary>
            /// Endpoint to save a calendar slot.
            /// </summary>
            public const string SaveCalendarSlot = "/Booking/SaveCalendarSlot";

            /// <summary>
            /// Endpoint to retrieve experience information.
            /// </summary>
            public const string GetExperienceInfo = "/Booking/GetExperienceInfo";
        }
    }
}