using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightData.Data;
using FlightData.Models;
using FlightApi.DTOs;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlights()
        {
            var flights = await _context.Flights
                .Select(f => new FlightDto
                {
                    Id = f.Id,
                    Origin = f.Origin,
                    DepartureTime = f.DepartureTime,
                    Destination = f.Destination
                })
                .ToListAsync();

            return Ok(flights);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            return new FlightDto
            {
                Id = flight.Id,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime
            };
        }

        [HttpGet("search")]
        public async Task<ActionResult<object>> Search(
            string? origin = null,
            string? destination = null,
            DateTime? departureDate = null,
            DateTime? returnDate = null,
            string? tripType = null,
            int? passengers = null,
            bool? directOnly = null,
            string? cabinClass = null)
        {
 
            var baseQuery = _context.Flights.AsQueryable();

            if (!string.IsNullOrWhiteSpace(origin))
            {
                baseQuery = baseQuery.Where(f => EF.Functions.Like(f.Origin, $"%{origin}%"));
            }
            if (!string.IsNullOrWhiteSpace(destination))
            {
                baseQuery = baseQuery.Where(f => EF.Functions.Like(f.Destination, $"%{destination}%"));
            }
            if (departureDate.HasValue)
            {
                var d = departureDate.Value.Date;
                baseQuery = baseQuery.Where(f => f.DepartureTime >= d && f.DepartureTime < d.AddDays(1));
            }
            if (directOnly == true)
            {
                baseQuery = baseQuery.Where(f => f.IsDirect);
            }
            if (passengers.HasValue)
            {
                baseQuery = baseQuery.Where(f => (f.Capacity - f.Bookings.Count() >= passengers.Value));
            }
            if (!string.IsNullOrWhiteSpace(cabinClass))
            {
                baseQuery = baseQuery.Where(f => EF.Functions.Like(f.CabinClass, $"%{cabinClass}%"));
            }

            var outboundQuery = baseQuery.OrderBy(f => f.DepartureTime).Take(10);

            var outbound = await outboundQuery
                        .Select(f => new FlightDto
                        {
                            Id = f.Id,
                            Origin = f.Origin,
                            Destination = f.Destination,
                            DepartureTime = f.DepartureTime,
                            Availableseats = f.Capacity - f.Bookings.Count()
                        }).ToListAsync();

            if (tripType == "roundtrip" && returnDate.HasValue)
            {
                var returnQuery = _context.Flights.AsQueryable();

                if (!string.IsNullOrWhiteSpace(destination))
                {
                    returnQuery = returnQuery.Where(f => EF.Functions.Like(f.Origin, $"%{destination}%"));
                }
                if (!string.IsNullOrWhiteSpace(origin))
                {
                    returnQuery = returnQuery.Where(f => EF.Functions.Like(f.Destination, $"%{origin}%"));
                }
                if (directOnly == true)
                {
                    returnQuery = returnQuery.Where(f => f.IsDirect);
                }
                if (passengers.HasValue)
                {
                    returnQuery = returnQuery.Where(f => (f.Capacity - f.Bookings.Count() >= passengers.Value));
                }
                if (!string.IsNullOrWhiteSpace(cabinClass))
                {
                    returnQuery = returnQuery.Where(f => EF.Functions.Like(f.CabinClass, $"%{cabinClass}%"));
                }

                var rd = returnDate.Value.Date;
                returnQuery = returnQuery.Where(f => f.DepartureTime >= rd && f.DepartureTime < rd.AddDays(1));

                var inboundQuery = returnQuery.OrderBy(f => f.DepartureTime).Take(10);

                var inbound = await inboundQuery
                            .Select(f => new FlightDto
                            {
                                Id = f.Id,
                                Origin = f.Origin,
                                Destination = f.Destination,
                                DepartureTime = f.DepartureTime,
                                Availableseats = f.Capacity - f.Bookings.Count()
                            }).ToListAsync();

                if (inbound != null)
                    return Ok(new { outbound, inbound });
            }
            return Ok(new { outbound });
        }

        [HttpPost]
        public async Task<ActionResult<FlightDto>> CreateFlight(CreateFlightDto dto)
        {
            // Mapeamos dto -> Entidad
            var flight = new Flight
            {
                Origin = dto.Origin,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime
            };

            // Lo añadimos y gardamos
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Mapear Entidad -> Dto de respuesta
            var result = new FlightDto
            {
                Id = flight.Id,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime
            };

            // Devolvemos OK!
            return CreatedAtAction(nameof(GetFlight), new { Id = flight.Id }, result);
        }

        [HttpPut("{id}")]
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

            // Guardamos los cambios en la bbdd
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
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