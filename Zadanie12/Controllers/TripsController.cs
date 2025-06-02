using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zadanie12.DTOs;
using Zadanie12.Services;

namespace Zadanie12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips([FromQuery] int page, [FromQuery] int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Strona i rozmiar strony muszą być większe niż 0");
            
            var result = await _tripService.GetTripsAsync(page, pageSize);
            return Ok(result);
            
        }
        
        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] AssignClientDto dto)
        {
            try
            {
                await _tripService.AssignClientToTripAsync(idTrip, dto);
                return Ok(new { message = "Client assigned to trip successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
