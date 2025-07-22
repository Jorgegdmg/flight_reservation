namespace FlightApi.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public PassengerDto Passenger { get; set; } = default!;
        public string SeatNumber { get; set; } = default!;
    }
}