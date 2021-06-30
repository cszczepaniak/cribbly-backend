using System;
using System.Threading.Tasks;
using System.Linq;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CribblyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ControllerBase
    {
        private readonly IDivisionService _divisionService;
        private readonly ILogger _logger;
        public DivisionController(IDivisionService divisionService, ILogger logger)
        {
            _divisionService = divisionService;
            _logger = logger;
        }

        /// <summary>
        /// Create makes a new Division object.
        /// </summary>
        /// <param name="Division">The Division object that will be created</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Division division)
        {
            try
            {
                _logger.Information("Received request to create Division: {@Division}", division);
                await _divisionService.Create(division);
            }
            catch (Exception e)
            {
                _logger.Information(e.Message, "Failed to create Division: {@Division}", division);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
            return Ok(division);
        }
    }
}