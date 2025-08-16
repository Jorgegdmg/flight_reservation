using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightData.Data;
using FlightData.Models;
using FlightApi.DTOs;
using FlightApi.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FlightApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FlightsController(AppDbContext context)
        {
            _context = context;
        }

        // CRUD
        [HttpGet("GetFlights")]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlights()
        {
            var flights = await _context.Flights
                .Select(f => new FlightDto
                {
                    Id = f.Id,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    Capacity = f.Capacity,
                    AvailableSeats = f.Capacity - f.Bookings.Count(),
                    IsDirect = f.IsDirect,
                    CabinClass = f.CabinClass
                })
                .ToListAsync();

            return Ok(flights);
        }

        [HttpGet("debug/count")]
        public async Task<ActionResult> Count()
        {
            var total = await _context.Flights.CountAsync();
            return Ok(new { total });
        }


        [HttpGet("GetFlight/:{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            return new FlightDto
            {
                Id = flight.Id,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                Capacity = flight.Capacity,
                AvailableSeats = flight.Capacity - flight.Bookings.Count(), // calculado
                IsDirect = flight.IsDirect,
                CabinClass = flight.CabinClass
            };
        }

        [HttpGet("FlightSearch")]
        public async Task<ActionResult<object>> FlightSearch(
            string origin,
            string destination,
            DateTime departureDate,
            DateTime? returnDate,
            string tripType,
            int passengers,
            bool directOnly,
            string cabinClass)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (tripType.ToLower() != "roundtrip" && tripType.ToLower() != "oneway")
                return BadRequest("Tipo de viaje inválido.");

            // OUTBOUND
            var d = departureDate.Date;
            var outboundQuery = _context.Flights.AsQueryable()
                .Where(f => f.DepartureTime >= d && f.DepartureTime < d.AddDays(1))
                .Where(f => EF.Functions.Like(f.Origin, $"%{origin}%"))
                .Where(f => EF.Functions.Like(f.Destination, $"%{destination}%"))
                .Where(f => f.Capacity - f.Bookings.Count() >= passengers)
                .Where(f => EF.Functions.Like(f.CabinClass, $"%{cabinClass}%"));

            if (directOnly)
                outboundQuery = outboundQuery.Where(f => f.IsDirect);

            var outbound = await outboundQuery
                .OrderBy(f => f.DepartureTime)
                .Take(10)
                .Select(f => new FlightDto
                {
                    Id = f.Id,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    AvailableSeats = f.Capacity - f.Bookings.Count(),
                    CabinClass = f.CabinClass
                }).ToListAsync();

            // INBOUND (solo roundtrip)
            if (tripType.ToLower() == "roundtrip")
            {
                if (!returnDate.HasValue)
                    return BadRequest("Debe indicar returnDate para viajes de ida y vuelta.");

                var rd = returnDate.Value.Date;
                var inboundQuery = _context.Flights.AsQueryable()
                    .Where(f => f.DepartureTime >= rd && f.DepartureTime < rd.AddDays(1))
                    .Where(f => EF.Functions.Like(f.Origin, $"%{destination}%"))
                    .Where(f => EF.Functions.Like(f.Destination, $"%{origin}%"))
                    .Where(f => f.Capacity - f.Bookings.Count() >= passengers)
                    .Where(f => EF.Functions.Like(f.CabinClass, $"%{cabinClass}%"));

                if (directOnly)
                    inboundQuery = inboundQuery.Where(f => f.IsDirect);

                var inbound = await inboundQuery
                    .OrderBy(f => f.DepartureTime)
                    .Take(10)
                    .Select(f => new FlightDto
                    {
                        Id = f.Id,
                        Origin = f.Origin,
                        Destination = f.Destination,
                        DepartureTime = f.DepartureTime,
                        AvailableSeats = f.Capacity - f.Bookings.Count(),
                        CabinClass = f.CabinClass
                    }).ToListAsync();

                return Ok(new { outbound, inbound });
            }

            return Ok(new { outbound });
        }


        [HttpPost("CreateFlight")]
        public async Task<ActionResult<FlightDto>> CreateFlight(CreateFlightDto dto)
        {

            // Validaciones
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            if (dto.DepartureTime < DateTime.UtcNow)
                return BadRequest("La fecha de salida no puede ser anterior al momento actual.");

            // Asignar capacity por defecto si el cliente no lo puso (opcional)
            var capacity = dto.Capacity > 0 ? dto.Capacity : 180;


            // Mapeamos dto -> Entidad
            var flight = new Flight
            {
                Origin = dto.Origin,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                Capacity = capacity,
                IsDirect = dto.IsDirect,
                CabinClass = dto.CabinClass
            };

            // Lo añadimos y gardamos
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Calcular available seats (normalmente sin bookings => capacity)
            var availableSeats = capacity - await _context.Bookings.CountAsync(b => b.FlightId == flight.Id);

            // Mapear Entidad -> Dto de respuesta
            var result = new FlightDto
            {
                Id = flight.Id,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                AvailableSeats = availableSeats,
                IsDirect = dto.IsDirect,
                CabinClass = dto.CabinClass
            };

            // Devolvemos OK!
            return CreatedAtAction(nameof(GetFlight), new { Id = flight.Id }, result);
        }

        [HttpPut("UpdateFlight/:{id}")]
        public async Task<IActionResult> UpdateFlight(int id, CreateFlightDto dto)
        {
            // Buscamos el id
            var flight = await _context.Flights.FindAsync(id);
            // Si no lo encontramos devolvemos no encontrado
            if (flight == null) return NotFound();

            // Si lo encontramos actualizamos los registros
            flight.Origin = dto.Origin;
            flight.Destination = dto.Destination;
            flight.DepartureTime = dto.DepartureTime;
            flight.Capacity = dto.Capacity;
            flight.IsDirect = dto.IsDirect;
            flight.CabinClass = dto.CabinClass;

            // Guardamos los cambios en la bbdd
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("DeleteFlight/:{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            // Buscamos el id
            var flight = await _context.Flights.FindAsync(id);
            // Si no lo encontramos devolvemos no encontrado
            if (flight == null) return NotFound();

            // Si lo encontramos borramos
            _context.Flights.Remove(flight);
            // Actualizamos BBDD
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}