namespace FlightApi.DTOs
{
    public class PassengerDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}