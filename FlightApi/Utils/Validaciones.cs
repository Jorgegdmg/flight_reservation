using FlightApi.DTOs;

namespace FlightApi.Utils
{
    public class Validaciones : IValidaciones
    {
        public bool IsAValidSearch(SearchRequest request)
        {
            
            if (string.IsNullOrWhiteSpace(request.Origin) ||
                string.IsNullOrWhiteSpace(request.Destination) ||
                !request.DepartureDate.HasValue ||
                !request.Passengers.HasValue ||
                string.IsNullOrWhiteSpace(request.CabinClass) ||
                !request.DirectOnly.HasValue)
            {
                return false;
            }

            if (request.TripType?.ToLower() == "roundtrip" &&
                !request.ReturnDate.HasValue)
            {
                return false;
            }

            return true;
        }

    }
}
