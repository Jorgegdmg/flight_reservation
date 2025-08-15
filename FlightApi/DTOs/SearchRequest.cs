namespace FlightApi.DTOs
{
    public class SearchRequest
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? TripType { get; set; }
        public int? Passengers { get; set; }
        public bool? DirectOnly { get; set; }
        public string? CabinClass { get; set; }
    }
}