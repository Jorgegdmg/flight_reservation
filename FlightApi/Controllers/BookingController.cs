using FlightApi.DTOs;
using FlightData.Data;
using FlightData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var list = await _context.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    FlightId = b.FlightId,
                    Passenger = new PassengerDto
                    {
                        Id = b.Passenger.Id,
                        Name = b.Passenger.Name,
                        Email = b.Passenger.Email
                    },
                    SeatNumber = b.SeatNumber

                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            // Buscamos la reserva
            var b = await _context.Bookings
                .Include(bk => bk.Passenger)
                .FirstOrDefaultAsync(bk => bk.Id == id);

            if (b == null) return NotFound();

            // Si la encontramos la devolvemos
            return new BookingDto
            {
                Id = b.Id,
                FlightId = b.FlightId,
                Passenger = new PassengerDto
                {
                    Id = b.Passenger.Id,
                    Name = b.Passenger.Name,
                    Email = b.Passenger.Email
                },
                SeatNumber = b.SeatNumber
            };
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
        {
            // Validamos la existencia del vuelo y del pasajero
            var flightExists = await _context.Flights.AnyAsync(f => f.Id == dto.FlightId!.Value);
            if (!flightExists) return BadRequest($"El vuelo con id {dto.FlightId!.Value} no existe.");

            var passengerExists = await _context.Passengers.AnyAsync(p => p.Id == dto.PassengerId!.Value);
            if (!passengerExists) return BadRequest($"El pasajero con id {dto.PassengerId!.Value} no existe.");

            // Creamos la reserva (mapeamos el DTO)
            var booking = new Booking
            {
                FlightId = dto.FlightId!.Value,
                PassengerId = dto.PassengerId!.Value,
                SeatNumber = dto.SeatNumber
            };

            // Guardamos cambios en BBDD
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Cargamos el pasajero para la respuesta
            await _context.Entry(booking).Reference(b => b.Passenger).LoadAsync();

            var result = new BookingDto
            {
                Id = booking.Id,
                FlightId = booking.FlightId,
                Passenger = new PassengerDto
                {
                    Id = booking.Passenger.Id,
                    Name = booking.Passenger.Name,
                    Email = booking.Passenger.Email
                },
                SeatNumber = booking.SeatNumber
            };

            // Devolvemos el objeto
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, CreateBookingDto dto)
        {
            // Buscamos la reserva
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            // Validaciones
            if (!await _context.Flights.AnyAsync(f => f.Id == dto.FlightId!.Value))
                return BadRequest($"El vuelo con el id {dto.FlightId!.Value} no existe.");

            if (!await _context.Passengers.AnyAsync(p => p.Id == dto.PassengerId!.Value))
                return BadRequest($"El pasajero con el id {dto.PassengerId!.Value} no existe.");

            // Si la encontramos hacemos el update
            booking.FlightId = dto.FlightId!.Value;
            booking.PassengerId = dto.PassengerId!.Value;
            booking.SeatNumber = dto.SeatNumber;

            // Guardamos en BBDD
            await _context.SaveChangesAsync();

            // Devolvemos OK
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            // Buscamos la reserva
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            // Si la encontramos borramos
            _context.Bookings.Remove(booking);

            // Guardamos en BBDD 
            await _context.SaveChangesAsync();

            // Devolvemos OK
            return NoContent();
        }
        
        [HttpGet("error")]
        public IActionResult ThrowError() => throw new Exception("Prueba de middleware de errores");









    } 
}