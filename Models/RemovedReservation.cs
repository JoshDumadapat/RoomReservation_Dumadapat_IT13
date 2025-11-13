namespace RoomReservation_Dumadapat_IT13.Models
{
    public class RemovedReservation
    {
        public int RemovedID { get; set; }
        public int? ReservationID { get; set; }
        public string? CustomerFname { get; set; }
        public string? CustomerLname { get; set; }
        public string? ContactNum { get; set; }
        public string? Email { get; set; }
        public string? RoomType { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Status { get; set; }
        public string? ReasonNote { get; set; }
        public int? AdminID { get; set; }
        public string? RemovedBy { get; set; }
        public DateTime? DateRemoved { get; set; }
        public decimal RoomPrice { get; set; }
        public string? AdminName { get; set; } // For display purposes
    }
}

