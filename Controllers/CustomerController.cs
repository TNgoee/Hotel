using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IAccountService _accountService;
    private readonly DropboxService _dropboxService;

    public CustomerController(ICustomerService customerService, DropboxService dropboxService, IAccountService accountService)
    {
        _customerService = customerService;
        _dropboxService = dropboxService;
        _accountService = accountService;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Customer>>> Get()
    {
        var customers = await _customerService.GetAllCustomer();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Customer>> GetById(string id)
    {
        var customers = await _customerService.GetCustomerById(id);
        if (customers == null)
        {
            return NotFound("customers not found");
        }
        return Ok(customers);
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] CustomerCreateDto customerCreateDto, IFormFile imageFile)
    {
        if (customerCreateDto == null || imageFile == null)
        {
            return BadRequest("Invalid data");
        }

        // Kiểm tra xem tài khoản có tồn tại không
        var account = await _accountService.GetAccountById(customerCreateDto.AccountId);
        if (account == null)
        {
            return BadRequest("Account does not exist");
        }

        // Upload ảnh và lấy URL
        string imageUrl = await _dropboxService.UploadFileToAvatarAsync(imageFile.OpenReadStream(), imageFile.FileName);

        var customer = new Customer
        {
            Name = customerCreateDto.Name,
            Birthday = customerCreateDto.Birthday,
            AccountId = customerCreateDto.AccountId,
            Avatar = imageUrl
        };

        // Gán tài khoản vào Customer
        customer.Account = account;

        // Tạo khách hàng mới và truyền imageFile vào đây
        await _customerService.CreateCustomer(customer, imageFile);  // Truyền imageFile vào đây

        return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
    }


    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Patch(string id, [FromForm] CustomerEditDto customerEditDto, IFormFile? imageFile)
    {
        var customer = await _customerService.GetCustomerById(id);
        if (customer == null)
        {
            return NotFound("Customer not found");
        }

        // Cập nhật các thuộc tính có giá trị mới
        if (!string.IsNullOrEmpty(customerEditDto.Name))
            customer.Name = customerEditDto.Name;

        customer.Birthday = customerEditDto.Birthday;

        if (!string.IsNullOrEmpty(customerEditDto.AccountId))
            customer.AccountId = customerEditDto.AccountId;

        // Xử lý cập nhật hình ảnh nếu có
        if (imageFile != null)
        {
            try
            {
                if (!string.IsNullOrEmpty(customer.Avatar))
                {
                    string dropboxPath = await _dropboxService.GetDropboxPathFromUrl(customer.Avatar);
                    if (!string.IsNullOrEmpty(dropboxPath))
                    {
                        await _dropboxService.DeleteFileAsync(dropboxPath);
                    }
                }

                string newImageUrl = await _dropboxService.UploadFileAsync(imageFile.OpenReadStream(), imageFile.FileName);
                customer.Avatar = newImageUrl;
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update image on Dropbox: {ex.Message}");
            }
        }

        await _customerService.UpdateCustomer(id, customer);
        return Ok("Customer updated successfully");
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id)
    {
        var customer = await _customerService.GetCustomerById(id);
        if (customer == null)
        {
            return NotFound("customer not found");
        }

        if (!string.IsNullOrEmpty(customer.Avatar))
        {
            try
            {

                string dropboxPath = await _dropboxService.GetDropboxPathFromUrl(customer.Avatar);


                if (!string.IsNullOrEmpty(dropboxPath))
                {
                    await _dropboxService.DeleteFileAsync(dropboxPath);
                }
                else
                {
                    return BadRequest("Failed to retrieve Dropbox path from URL.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete image from Dropbox: {ex.Message}");
            }
        }

        await _customerService.DeleteCustomer(id);
        return Ok("Customer deleted successfully");
    }

}