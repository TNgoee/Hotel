using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _account;
    public AccountController(IAccountService account)
    {
        _account = account;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Account>>> Get()
    {
        var account = await _account.GetAllAccount();
        return Ok(account);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Account>> GetById(string id)
    {
        var account = await _account.GetAccountById(id);
        if (account == null)
        {
            return NotFound("account not found");
        }
        return Ok(account);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] AccountCreateDto accountCreateDto)
    {
        if (accountCreateDto == null)
        {
            return BadRequest("Invalid data");

        }
        var account = new Account
        {
            UserName = accountCreateDto.UserName,
            PasswordHash = accountCreateDto.PasswordHash,
            RoleId = accountCreateDto.RoleId,
            CreatedAt = accountCreateDto.CreatedAt,
            UpdatedAt = accountCreateDto.UpdatedAt,
        };
        await _account.CreateAccount(account);
        return CreatedAtAction(nameof(Get), new { account });
    }


    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Patch(string id, [FromForm] AccountEditDto accountEditDto)
    {
        var account = await _account.GetAccountById(id);
        if (account == null)
        {
            return NotFound("Account not found");
        }
        if (!string.IsNullOrEmpty(accountEditDto.UserName))
            account.UserName = accountEditDto.UserName;
        if (!string.IsNullOrEmpty(accountEditDto.PasswordHash))
            account.PasswordHash = accountEditDto.PasswordHash;
        if (!string.IsNullOrEmpty(accountEditDto.RoleId))
            account.RoleId = accountEditDto.RoleId;
        account.UpdatedAt = DateTime.UtcNow;
        await _account.UpdateAccount(id, account);
        return Ok("Account updated successfully");
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var isDeleted = await _account.DeleteAccount(id);
        if (!isDeleted)
        {
            return NotFound("Account not found");

        }
        return NoContent();
    }

}