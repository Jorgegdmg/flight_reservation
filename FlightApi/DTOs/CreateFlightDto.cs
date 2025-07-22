namespace FlightApi.DTOs
{
    public class CreateFlightDto
    {
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
    }
}