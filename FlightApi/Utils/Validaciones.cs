using FlightApi.DTOs;

namespace FlightApi.Utils
{
    public class Validaciones : IValidaciones
    {
        public bool IsAValidSearch(SearchRequest request, string tripType)
        {
            // Origen y destino no pueden ser iguales
            if (request.Origin?.ToLower() == request.Destination?.ToLower())
                return false;

            // Si es roundtrip, la fecha de vuelta debe ser posterior a la de ida
            if (tripType.ToLower() == "roundtrip" &&
                request.ReturnDate.HasValue &&
                request.ReturnDate <= request.DepartureDate)
                return false;

            return true;
        }
    }
}

