using System;
using System.Collections.Generic;

namespace FlightData.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}