using System.ComponentModel.DataAnnotations;

namespace FlightApi.DTOs
{
    public class CreatePassengerDto
    {
        [Required(ErrorMessage = "El nombre es un campo obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe contener entre 2 y 50 caracteres.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "El email es un campo obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El email debe contener entre 2 y 50 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = default!;
    }
}