using FlightApi.DTOs;
using FlightData.Data;
using FlightData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PassengersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetPassengers")]
        public async Task<ActionResult<IEnumerable<PassengerDto>>> GetPassengers()
        {
            var list = await _context.Passengers
                .Select(p => new PassengerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("GetPassenger/:{id}")]
        public async Task<ActionResult<PassengerDto>> GetPassenger(int id)
        {
            var p = await _context.Passengers.FindAsync(id);
            if (p == null) return NotFound();

            return new PassengerDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email
            };
        }

        [HttpPost("CreatePassenger")]
        public async Task<ActionResult<PassengerDto>> CreatePassenger(CreatePassengerDto dto)
        {
            var passenger = new Passenger
            {
                Name = dto.Name,
                Email = dto.Email
            };

            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            var result = new PassengerDto
            {
                Id = passenger.Id,
                Name = passenger.Name,
                Email = passenger.Email
            };

            return CreatedAtAction(nameof(GetPassenger), new { id = passenger.Id }, result);

        }
        [HttpPut("UpdatePassenger/:{id}")]
        public async Task<ActionResult<PassengerDto>> UpdatePassenger(int id, CreatePassengerDto dto)
        {
            // Buscamos al pasajero
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();

            // Si le encontramos hacemos el update
            passenger.Name = dto.Name;
            passenger.Email = dto.Email;
            // Guardamos en la BBDD
            await _context.SaveChangesAsync();
            // Devolvemos OK!
            return NoContent();
        }

        [HttpDelete("DeletePassenger/:{id}")]
        public async Task<ActionResult<PassengerDto>> DeletePassenger(int id)
        {
            // Buscamos el pasajero
            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null) return NotFound();

            // Si le encontramos le borramos
            _context.Passengers.Remove(passenger);
            // Guardamos cambios en la BBDD
            await _context.SaveChangesAsync();
            // Devolvemos OK
            return NoContent();
        }
    }
}