using Heron.MudCalendar;
using MudBlazor;
using Psybook.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Psybook.Objects.DbModels
{
    public class CalendarSlot : CalendarItem
    {
        public new Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Title { get; set; }
        public required string Location { get; set; }
        public required Color Color { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ContactNumber { get; set; }
        public required string FirstLineAddress { get; set; }
        public required string Postcode { get; set; }
        public required BookingExperience BookingExperience { get; set; }
        
        // Booking Status Management
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        
        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? Notes { get; set; }
        
        // Computed Properties
        [NotMapped]
        public bool IsActive => Status == BookingStatus.Confirmed || Status == BookingStatus.Pending;
        
        [NotMapped]
        public bool IsCancellable => Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;
        
        [NotMapped]
        public bool IsModifiable => Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;
        
        [NotMapped]
        public string StatusDisplayName => Status switch
        {
            BookingStatus.Pending => "Pending Confirmation",
            BookingStatus.Confirmed => "Confirmed",
            BookingStatus.Cancelled => "Cancelled",
            BookingStatus.Completed => "Completed",
            BookingStatus.NoShow => "No Show",
            BookingStatus.OnHold => "On Hold",
            _ => "Unknown"
        };
        
        [NotMapped]
        public Color StatusColor => Status switch
        {
            BookingStatus.Pending => Color.Warning,
            BookingStatus.Confirmed => Color.Success,
            BookingStatus.Cancelled => Color.Error,
            BookingStatus.Completed => Color.Info,
            BookingStatus.NoShow => Color.Dark,
            BookingStatus.OnHold => Color.Secondary,
            _ => Color.Default
        };
    }
}