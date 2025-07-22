using System;
using System.Collections.Generic;

namespace FlightData.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = default!;
        public int PassengerId { get; set; }
        public Passenger Passenger { get; set; } = default!;
        public string SeatNumber { get; set; } = default!;
    }
}
