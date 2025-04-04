using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerDiscountController : ControllerBase
{

    private readonly ICustomDiscountService _customDiscount;
    public CustomerDiscountController(ICustomDiscountService customDiscountService)
    {
        _customDiscount = customDiscountService;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDiscount>>> Get()
    {
        var customDiscountService = await _customDiscount.GetAllCustomerDiscount();
        return Ok(customDiscountService);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Service>> GetById(string id)
    {
        var service = await _customDiscount.GetCustomerDiscountById(id);
        if (service == null)

        {
            return NotFound("customer discount not found");
        }
        return Ok(service);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] CustomerDiscountCreateDto customerDiscountCreateDto)
    {
        if (customerDiscountCreateDto == null)
        {
            return BadRequest("Invalid data");
        }

        var customerDiscount = new CustomerDiscount
        {
            CustomerId = customerDiscountCreateDto.CustomerId,
            DiscountId = customerDiscountCreateDto.DiscountId,
            IsUsed = customerDiscountCreateDto.IsUsed,
            ReceivedAt = customerDiscountCreateDto.ReceivedAt,
            UsedAt = customerDiscountCreateDto.UsedAt
        };

        await _customDiscount.CreateCustomerDiscount(customerDiscount);

        return CreatedAtAction(nameof(Get), new
        {
            customerDiscount
        });
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<CustomerDiscount>> Patch(string id, [FromForm] CustomerDiscountEditDto customerDiscountEditDto)
    {
        if (string.IsNullOrWhiteSpace(id) || customerDiscountEditDto == null)
        {
            return BadRequest("Invalid data");
        }
        var customerDiscount = await _customDiscount.GetCustomerDiscountById(id);
        if (customerDiscount == null)
        {
            return NotFound("Customer discount not found");
        }

        if (!string.IsNullOrEmpty(customerDiscountEditDto.CustomerId))
            customerDiscount.CustomerId = customerDiscountEditDto.CustomerId;

        if (!string.IsNullOrEmpty(customerDiscountEditDto.DiscountId))
            customerDiscount.DiscountId = customerDiscountEditDto.DiscountId;

        customerDiscount.IsUsed = customerDiscountEditDto.IsUsed;

        if (customerDiscountEditDto.ReceivedAt != default)
            customerDiscount.ReceivedAt = customerDiscountEditDto.ReceivedAt;

        if (customerDiscountEditDto.UsedAt.HasValue)
            customerDiscount.UsedAt = customerDiscountEditDto.UsedAt.Value;

        var isUpdated = await _customDiscount.UpdateCustomerDiscount(id, customerDiscount);
        if (!isUpdated)
        {
            return NotFound("Failed to update customer discount");
        }

        return Ok(customerDiscount);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var isDeleted = await _customDiscount.DeleteCustomerDiscount(id);
        if (!isDeleted)
        {
            return NotFound("Customer Discount not found");

        }
        return NoContent();
    }
}