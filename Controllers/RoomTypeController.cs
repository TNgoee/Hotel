using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RoomTypeController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;
    private readonly DropboxService _dropboxService;
    public RoomTypeController(IRoomTypeService roomTypeService, DropboxService dropboxService)
    {
        _dropboxService = dropboxService;
        _roomTypeService = roomTypeService;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomType>>> Get()
    {
        var roomType = await _roomTypeService.GetAllRoomType();
        return Ok(roomType);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RoomType>> GetById(string id)
    {
        var roomType = await _roomTypeService.GetRoomTypeById(id);
        if (roomType == null)
        {
            return BadRequest("RoomType not found");
        }
        return Ok(roomType);
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] RoomTypeCreateDto roomTypeCreateDto, IFormFile imageFile)
    {
        if (roomTypeCreateDto == null || imageFile == null)

        {
            return BadRequest("Invalid data");
        }
        string imageUrl = await _dropboxService.UploadFileToRoomServiceAsync(imageFile.OpenReadStream(), imageFile.FileName);
        var roomType = new RoomType
        {
            RoomTypeName = roomTypeCreateDto.RoomTypeName,
            Description = roomTypeCreateDto.Description,
            Price = roomTypeCreateDto.Price,
            RoomCount = roomTypeCreateDto.RoomCount,
            ServiceIds = roomTypeCreateDto.ServiceIds,
            Img = imageUrl
        };
        await _roomTypeService.CreateRoomType(roomType, imageFile);
        return CreatedAtAction(nameof(Get), new { roomType });
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Patch(string id, [FromForm] RoomTypeUpdateDto roomTypeUpdateDto, IFormFile? imageFile)
    {
        var roomType = await _roomTypeService.GetRoomTypeById(id);
        if (roomType == null)
        {
            return BadRequest("RoomType not found");
        }
        if (!string.IsNullOrEmpty(roomTypeUpdateDto.RoomTypeName))
            roomType.RoomTypeName = roomTypeUpdateDto.RoomTypeName;
        if (!string.IsNullOrEmpty(roomTypeUpdateDto.Description))
            roomType.Description = roomTypeUpdateDto.Description;
        if (roomTypeUpdateDto.Price > 0)
            roomType.Price = roomTypeUpdateDto.Price;
        if (roomTypeUpdateDto.RoomCount >= 0)
            roomType.RoomCount = roomTypeUpdateDto.RoomCount;

        if (roomTypeUpdateDto.ServiceIds != null && roomTypeUpdateDto.ServiceIds.Any())
            roomType.ServiceIds = roomTypeUpdateDto.ServiceIds;

        if (imageFile != null)
        {
            try
            {
                if (!string.IsNullOrEmpty(roomType.Img))
                {
                    string dropboxPath = await _dropboxService.GetDropboxPathFromUrl(roomType.Img);
                    if (!string.IsNullOrEmpty(dropboxPath))
                    {
                        await _dropboxService.DeleteFileAsync(dropboxPath);
                    }
                }
                string newImageUrl = await _dropboxService.UploadFileToRoomServiceAsync(imageFile.OpenReadStream(), imageFile.FileName);
                roomType.Img = newImageUrl;
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update image on Dropbox: {ex.Message}");
            }
        }
        var updateResult = await _roomTypeService.UpdateRoomType(id, roomType);
        if (updateResult)
        {
            return Ok("RoomType updated successfully");
        }
        else
        {
            return StatusCode(500, "An error occurred while updating the RoomType");
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id)
    {
        var roomType = await _roomTypeService.GetRoomTypeById(id);
        if (roomType == null)
        {
            return NotFound("Room Type not found");
        }
        if (!string.IsNullOrEmpty(roomType.Img))
        {
            try
            {
                string dropboxPath = await _dropboxService.GetDropboxPathFromUrl(roomType.Img);

                if (!string.IsNullOrEmpty(dropboxPath))
                {
                    await _dropboxService.DeleteFileAsync(dropboxPath);
                }
                else
                {
                    return BadRequest("Failed to retrieve Dropbox path from URL");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete image from Dropbox: {ex.Message}");
            }
        }
        await _roomTypeService.DeleteRoomType(id);
        return Ok("RoomType deleted successfully");
    }
}