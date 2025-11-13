namespace RoomReservation_Dumadapat_IT13.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public string? CustomerFname { get; set; }
        public string? CustomerLname { get; set; }
        public string? ContactNum { get; set; }
        public string? Email { get; set; }
        public string? RoomType { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public decimal RoomPrice { get; set; }
    }
}

