using System.ComponentModel.DataAnnotations;

namespace FlightApi.DTOs
{
    public class CreateFlightDto
    {
        [Required(ErrorMessage = "El origen es un campo obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El origen debe contener entre 3 y 100 caracteres.")]
        public string Origin { get; set; } = default!;

        [Required(ErrorMessage = "El destino es un campo obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El destino debe contener entre 3 y 100 caracteres.")]
        public string Destination { get; set; } = default!;

        [Required(ErrorMessage = "La hora de salida es obligatoria.")]
        public DateTime DepartureTime { get; set; }
    }
}