namespace FlightApi.DTOs
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
        public int Capacity { get; set; } = default!;
        public int AvailableSeats { get; set; } = default!;
        public bool IsDirect { get; set; } = true;
        public string CabinClass { get; set; } = default!;
    }
}