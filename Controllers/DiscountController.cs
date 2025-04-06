using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountService _discountService;
    private readonly ICloudinaryService _cloudinaryService;  // Thay DropboxService bằng ICloudinaryService

    public DiscountController(IDiscountService discountService, ICloudinaryService cloudinaryService)
    {
        _discountService = discountService;
        _cloudinaryService = cloudinaryService;  // Sử dụng ICloudinaryService
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Discount>>> Get()
    {
        var discount = await _discountService.GetAllDiscount();
        return Ok(discount);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Discount>> GetById(string id)
    {
        var discount = await _discountService.GetDiscountById(id);
        if (discount == null)
        {
            return NotFound("Discount not found");
        }
        return Ok(discount);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] DiscountCreateDto discountCreateDto, IFormFile imageFile)
    {
        if (discountCreateDto == null || imageFile == null)
        {
            return BadRequest("Invalid data");
        }

        // Upload ảnh lên Cloudinary thay vì Dropbox
        string imageUrl = await _cloudinaryService.UploadFileAsync(imageFile.OpenReadStream(), imageFile.FileName);
        Console.WriteLine($"🔹 Uploaded file to Cloudinary: {imageUrl}");

        var discount = new Discount
        {
            NameDiscount = discountCreateDto.NameDiscount,
            Price = discountCreateDto.Price,
            Description = discountCreateDto.Description,
            MinAmount = discountCreateDto.MinAmount,
            MaxQuantity = discountCreateDto.MaxQuantity,
            StartDate = discountCreateDto.StartDate,
            EndDate = discountCreateDto.EndDate,
            Img = imageUrl
        };

        await _discountService.CreateDiscount(discount, imageFile);

        return CreatedAtAction(nameof(Get), new { discount });
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Patch(string id, [FromForm] DiscountEditDto discountUpdateDto, IFormFile? imageFile)
    {
        var discount = await _discountService.GetDiscountById(id);
        if (discount == null)
        {
            return NotFound("Discount not found");
        }

        // Cập nhật các thuộc tính có giá trị mới
        if (!string.IsNullOrEmpty(discountUpdateDto.NameDiscount))
            discount.NameDiscount = discountUpdateDto.NameDiscount;

        if (discountUpdateDto.MinAmount.HasValue)
            discount.MinAmount = discountUpdateDto.MinAmount.Value;

        if (discountUpdateDto.MaxQuantity.HasValue)
            discount.MaxQuantity = discountUpdateDto.MaxQuantity.Value;

        if (discountUpdateDto.StartDate.HasValue)
            discount.StartDate = discountUpdateDto.StartDate.Value;

        if (discountUpdateDto.EndDate.HasValue)
            discount.EndDate = discountUpdateDto.EndDate.Value;

        // Xử lý cập nhật hình ảnh nếu có
        if (imageFile != null)
        {
            try
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(discount.Img))
                {
                    await _cloudinaryService.DeleteFileAsync(discount.Img); // Xóa ảnh cũ từ Cloudinary
                }

                // Upload ảnh mới lên Cloudinary
                string newImageUrl = await _cloudinaryService.UploadFileAsync(imageFile.OpenReadStream(), imageFile.FileName);
                discount.Img = newImageUrl;
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update image on Cloudinary: {ex.Message}");
            }
        }

        await _discountService.UpdateDiscount(id, discount);
        return Ok("Discount updated successfully");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id)
    {
        var discount = await _discountService.GetDiscountById(id);
        if (discount == null)
        {
            return NotFound("Discount not found");
        }

        if (!string.IsNullOrEmpty(discount.Img))
        {
            try
            {
                // Xóa ảnh từ Cloudinary nếu có
                await _cloudinaryService.DeleteFileAsync(discount.Img);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete image from Cloudinary: {ex.Message}");
            }
        }

        await _discountService.DeleteDiscount(id);
        return Ok("Discount deleted successfully");
    }
}
