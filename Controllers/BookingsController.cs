using Microsoft.AspNetCore.Mvc;
using BookingRoomCampus.Data;
using BookingRoomCampus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookingRoomCampus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/bookings
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(
            string? nama,
            string? ruangan,
            string? status,
            string? sortBy)

        {
            var query = _context.Bookings.AsQueryable();

            //filter nama
            if (!string.IsNullOrEmpty(nama))
            {
                query = query.Where(b => b.NamaPeminjam.Contains(nama));
            }

            //filter ruangan
            if (!string.IsNullOrEmpty(ruangan))
            {
                query = query.Where(b => b.Ruangan.Contains(ruangan));
            }

            //filter status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }

            //sorting
            if (sortBy == "tanggal")
            {
                query = query.OrderBy(b => b.Tanggal);
            }

            return await query.ToListAsync();
        }


        //GET: api/bookings
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        //PUT: api/bookings
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBooking(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PATCH: api/bookings/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/bookings
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //POST: api/bookings
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            // Validasi Format Jam
            if (!TimeSpan.TryParse(booking.JamMulai, out var jamMulai) ||
            !TimeSpan.TryParse(booking.JamSelesai, out var jamSelesai))
            {
                return BadRequest("format jam tidak valid, gunakan HH:MM");
            }

            //Validasi jam
            if (jamMulai >= jamSelesai)
            {
                return BadRequest("Jam tidak valid, jam mulai harus lebih dulu dari jam selesai");
            }

            // ambil booking yang sama ruangan dan tanggal
            var existingBookings = await _context.Bookings
                .Where(b => b.Ruangan == booking.Ruangan &&
                            b.Tanggal.Date == booking.Tanggal.Date)
                .ToListAsync();

            // cek bentrok di memory (bukan SQL)
            var isBentrok = existingBookings.Any(b =>
                TimeSpan.Parse(b.JamMulai) < jamSelesai &&
                TimeSpan.Parse(b.JamSelesai) > jamMulai
            );

            if (isBentrok)
            {
                return BadRequest("Ruangan sudah dibooking pada jam tersebut");
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }
    }
}