namespace FlightApi.DTOs
{
    public class CreatePassengerDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}