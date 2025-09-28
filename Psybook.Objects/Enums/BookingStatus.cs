namespace Psybook.Objects.Enums
{
    /// <summary>
    /// Represents the current status of a booking in the system.
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Booking has been created but is awaiting confirmation (e.g., payment processing).
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Booking has been confirmed and is active.
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Booking has been cancelled by the customer or staff.
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// The experience has been completed.
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Customer did not show up for the booked experience.
        /// </summary>
        NoShow = 4,

        /// <summary>
        /// Booking is on hold pending further action (e.g., payment issues).
        /// </summary>
        OnHold = 5
    }
}