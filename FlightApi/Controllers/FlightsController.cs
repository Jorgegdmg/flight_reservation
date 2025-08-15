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
        private readonly IValidaciones _validaciones;

        public FlightsController(AppDbContext context, IValidaciones validaciones)
        {
            _context = context;
            _validaciones = validaciones;
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
                    Destination = f.Destination,
                    DepartureTime = f.DepartureTime,
                    Capacity = f.Capacity,
                    AvailableSeats = f.Capacity - f.Bookings.Count(), // calculado
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
                DepartureTime = flight.DepartureTime,
                Capacity = flight.Capacity,
                AvailableSeats = flight.Capacity - flight.Bookings.Count(), // calculado
                IsDirect = flight.IsDirect,
                CabinClass = flight.CabinClass
            };
        }

        [HttpGet("search")]
        public async Task<ActionResult<object>> Search(
            string? origin = null,
            string? destination = null,
            DateTime? departureDate = null,
            DateTime? returnDate = null,
            string tripType = "roundtrip",
            int? passengers = null,
            bool? directOnly = null,
            string? cabinClass = null)
        {
 
            var request = new SearchRequest
            {
                Origin = origin,
                Destination = destination,
                DepartureDate = departureDate,
                ReturnDate = returnDate,
                Passengers = passengers,
                DirectOnly = directOnly,
                CabinClass = cabinClass
            };

            if (!_validaciones.IsAValidSearch(request))
            {
                return BadRequest("Parámetros inválidos.");
            }

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
                baseQuery = baseQuery.Where(f => f.CabinClass == cabinClass);
            }

            var outboundQuery = baseQuery.OrderBy(f => f.DepartureTime).Take(10);

            var outbound = await outboundQuery
                        .Select(f => new FlightDto
                        {
                            Id = f.Id,
                            Origin = f.Origin,
                            Destination = f.Destination,
                            DepartureTime = f.DepartureTime,
                            AvailableSeats = f.Capacity - f.Bookings.Count(),
                            CabinClass = f.CabinClass
                        }).ToListAsync();

            if (string.Equals(tripType, "roundtrip", StringComparison.OrdinalIgnoreCase))
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
                    returnQuery = returnQuery.Where(f => f.CabinClass == cabinClass);
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
                                AvailableSeats = f.Capacity - f.Bookings.Count()
                            }).ToListAsync();

                if (inbound != null)
                    return Ok(new { outbound, inbound });
            }
            return Ok(new { outbound });
        }

        [HttpGet]
        public async Task<ActionResult<object>> FlightSearchSearch(
            string? origin = null,
            string? destination = null,
            DateTime? departureDate = null,
            DateTime? returnDate = null,
            string tripType = "roundtrip",
            int? passengers = null,
            bool? directOnly = null,
            string? cabinClass = null)
        {

            var request = new SearchRequest
            {
                Origin = origin,
                Destination = destination,
                DepartureDate = departureDate,
                ReturnDate = returnDate,
                Passengers = passengers,
                DirectOnly = directOnly,
                CabinClass = cabinClass
            };

            if (!_validaciones.IsAValidSearch(request))
            {
                return BadRequest("Parámetros inválidos.");
            }

            var d   = departureDate.Value.Date;
            var rd  = returnDate.Value.Date;
            var baseQuery = _context.Flights.AsQueryable();

            baseQuery = baseQuery.Where(f => f.DepartureTime >= d && f.DepartureTime < d.AddDays(1))
                                 .Where(f => EF.Functions.Like(f.Origin, $"%{origin}%"))
                                 .Where(f => EF.Functions.Like(f.Destination, $"%{destination}%"))
                                 .Where(f => f.IsDirect)
                                 .Where(f => (f.Capacity - f.Bookings.Count() >= passengers.Value))
                                 .Where(f => f.CabinClass == cabinClass);
            var outboundQuery = baseQuery.OrderBy(f => f.DepartureTime).Take(10);

            var outbound = await outboundQuery
                        .Select(f => new FlightDto
                        {
                            Id = f.Id,
                            Origin = f.Origin,
                            Destination = f.Destination,
                            DepartureTime = f.DepartureTime,
                            AvailableSeats = f.Capacity - f.Bookings.Count(),
                            CabinClass = f.CabinClass
                        }).ToListAsync();

            if (tripType.ToLower() == "roundtrip")
            {
                baseQuery = baseQuery.Where(f => f.DepartureTime >= rd && f.DepartureTime < rd.AddDays(1));

                var inboundQuery = baseQuery.OrderBy(f => f.DepartureTime).Take(10);

                var inbound = await inboundQuery
                            .Select(f => new FlightDto
                            {
                                Id = f.Id,
                                Origin = f.Origin,
                                Destination = f.Destination,
                                DepartureTime = f.DepartureTime,
                                ReturnTime = f.ReturnTime,
                                AvailableSeats = f.Capacity - f.Bookings.Count(),
                                CabinClass = f.CabinClass
                            }).ToListAsync();

                if (inbound != null)
                    return Ok(new { outbound, inbound });
            }
            return Ok(new { outbound });
        }

        [HttpPost]
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
            flight.Capacity = dto.Capacity;
            flight.IsDirect = dto.IsDirect;
            flight.CabinClass = dto.CabinClass;

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