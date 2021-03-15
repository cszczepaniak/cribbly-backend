using System;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.Network;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.Debug("Received request to get division using id {id}", id);
            var d = await _divisionService.GetById(id);
            if (d != null)
            {
                return Ok(d);
            }
            _logger.Information("Request for division {id} returned no results", id);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDivisionRequest request)
        {
            try
            {
                _logger.Debug("Received request to create division {@request}", request);
                var createdId = await _divisionService.Create(request.Name, request.TeamIds);
                _logger.Debug("New division created with ID: {@id}", createdId);
                return Ok(createdId);
            }
            catch (Exception e)
            {
                _logger.Information("Failed to create division with request: {@request} -- MSG: {message}", request, e.Message);
                return StatusCode(500, $"Uh oh, bad time: {e.Message}");
            }
        }
    }
}