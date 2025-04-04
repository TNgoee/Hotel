using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ServiceController : ControllerBase
{
    private readonly IRoomServiceService _services;
    public ServiceController(IRoomServiceService services)
    {
        _services = services;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Service>>> Get()
    {
        var services = await _services.GetAllService();
        return Ok(services);
    }
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Service>> GetById(string id)
    {
        var service = await _services.GetServiceById(id);
        if (service == null)

        {
            return NotFound("service not found");
        }
        return Ok(service);
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] ServiceCreateDto serviceCreateDto)
    {
        if (serviceCreateDto == null)
        {
            return BadRequest("Invalid data");

        }
        var service = new Service
        {
            nameService = serviceCreateDto.nameService,
        };
        await _services.CreateService(service);
        return CreatedAtAction(nameof(Get), new { id = service.Id, name = service.nameService, });
    }
    [HttpPatch("{id}")]
    public async Task<ActionResult<Service>> Patch(string id, [FromForm] ServiceEditDto serviceEditDto)
    {
        if (string.IsNullOrWhiteSpace(id) || serviceEditDto == null)
        {
            return BadRequest("Invalid data");
        }
        var service = new Service
        {
            Id = id,
            nameService = serviceEditDto.nameService,
        };
        var isUpdated = await _services.UpdateService(id, service);
        if (!isUpdated)
        {
            return NotFound("Service not found");

        }
        return Ok(service);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var isDeleted = await _services.DeleteService(id);
        if (!isDeleted)
        {
            return NotFound("Service not found");

        }
        return NoContent();
    }
}

