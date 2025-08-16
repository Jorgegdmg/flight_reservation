using System.ComponentModel.DataAnnotations;

namespace FlightApi.DTOs
{
    public class SearchRequest
    {
        [Required(ErrorMessage = "Origin es obligatorio")]
        public string? Origin { get; set; }

        [Required(ErrorMessage = "Destination es obligatorio")]
        public string? Destination { get; set; }

        [Required(ErrorMessage = "DepartureDate es obligatorio")]
        public DateTime? DepartureDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Passengers es obligatorio")]
        [Range(1, 10, ErrorMessage = "Passengers debe estar entre 1 y 10")]
        public int? Passengers { get; set; }

        [Required(ErrorMessage = "CabinClass es obligatorio")]
        public string? CabinClass { get; set; }

        [Required(ErrorMessage = "DirectOnly es obligatorio")]
        public bool? DirectOnly { get; set; }
    }
}
