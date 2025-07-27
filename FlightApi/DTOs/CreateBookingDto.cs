using System.ComponentModel.DataAnnotations;

namespace FlightApi.DTOs
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "El id de pasajero es un campo obligatorio.")]
        public int? FlightId { get; set; }

        [Required(ErrorMessage = "El id de pasajero es un campo obligatorio.")]
        public int? PassengerId { get; set; }

        [Required(ErrorMessage = "El asiento es un campo obligatorio.")]
        [StringLength(4, MinimumLength = 2, ErrorMessage = "El asiento no debe contener mas de 4 caracteres")]
        public string SeatNumber { get; set; } = default!;

    }
}