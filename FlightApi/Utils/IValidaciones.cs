using FlightApi.DTOs;

namespace FlightApi.Utils
{
    public interface IValidaciones
    {
        bool IsAValidSearch(SearchRequest request);
    }
}