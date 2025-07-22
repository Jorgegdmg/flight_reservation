namespace FlightApi.DTOs
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
    }
}