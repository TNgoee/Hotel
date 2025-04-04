using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _role;
    public RoleController(IRoleService role)
    {
        _role = role;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Role>>> Get()
    {
        var role = await _role.GetAllRole();
        return Ok(role);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Role>> GetById(string id)
    {
        var role = await _role.GetRoleById(id);
        if (role == null)

        {
            return NotFound("role not found");
        }
        return Ok(role);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] RoleCreateDto roleCreateDto)
    {
        if (roleCreateDto == null)
        {
            return BadRequest("Invalid data");

        }
        var role = new Role
        {
            Name = roleCreateDto.Name,
            Description = roleCreateDto.Description,
        };
        await _role.CreateRole(role);
        return CreatedAtAction(nameof(Get), new { role });
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Role>> Patch(string id, [FromForm] RoleEditDto roleEditDto)
    {
        if (string.IsNullOrWhiteSpace(id) || roleEditDto == null)
        {
            return BadRequest("Invalid data");
        }
        var role = new Role
        {
            Id = id,
            Name = roleEditDto.Name,
            Description = roleEditDto.Description,
        };
        var isUpdated = await _role.UpdateRole(id, role);
        if (!isUpdated)
        {
            return NotFound("Role not found");
        }
        return Ok(role);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var isDeleted = await _role.DeleteRole(id);
        if (!isDeleted)
        {
            return NotFound("Role not found");
        }
        return NoContent();
    }
}