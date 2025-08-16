using FlightApi.DTOs;

namespace FlightApi.Utils
{
    public interface IValidaciones
    {
        public bool IsAValidSearch(SearchRequest request, string tripType);
    }
}