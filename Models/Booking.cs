using System;

namespace BookingRoomCampus.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string NamaPeminjam { get; set; }
        public string Ruangan { get; set; }
        public DateTime Tanggal { get; set; }
        public string JamMulai { get; set; }
        public string JamSelesai { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
