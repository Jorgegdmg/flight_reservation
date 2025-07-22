namespace FlightApi.DTOs
{
    public class CreateBookingDto
    {
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public string SeatNumber { get; set; } = default!;

    }
}