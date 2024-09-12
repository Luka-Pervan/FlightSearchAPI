using FlightSearchAPI.Models;
using FlightSearchAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightSearchAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly FlightSearchService _flightSearchService;

        public FlightsController(FlightSearchService flightSearchService)
        {
            _flightSearchService = flightSearchService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchFlights([FromBody] FlightSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var flights = await _flightSearchService.SearchFlightsAsync(request);
                return Ok(flights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching flight data: {ex.Message}");
            }
        }
    }
}
