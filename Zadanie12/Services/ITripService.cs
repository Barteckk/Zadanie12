using Zadanie12.DTOs;

namespace Zadanie12.Services;

public interface ITripService
{
    Task<TripsResponseDto> GetTripsAsync(int page, int pageSize);
    Task AssignClientToTripAsync(int idTrip, AssignClientDto dto);
}