using System;
using System.Collections.Generic;

namespace FlightData.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}