using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly DropboxService _dropboxService;
    public RoomController(IRoomService roomService, DropboxService dropboxService)
    {
        _roomService = roomService;
        _dropboxService = dropboxService;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Room>>> Get()
    {
        var rooms = await _roomService.GetAllRoom();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Room>> GetById(string id)
    {
        var rooms = await _roomService.GetRoomById(id);
        if (rooms == null)
        {
            return NotFound("Room not found");
        }
        return Ok(rooms);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] RoomCreateDto roomCreateDto, IFormFile imageFile)
    {


        if (roomCreateDto == null || imageFile == null)
        {
            return BadRequest("Invalid data");
        }

        string imageUrl = await _dropboxService.UploadFileToRoomAsync(imageFile.OpenReadStream(), imageFile.FileName);
        Console.WriteLine($"Uploaded file: {imageUrl}");

        var room = new Room
        {
            NameRoom = roomCreateDto.NameRoom,
            Floor = roomCreateDto.Floor,
            Status = roomCreateDto.Status,
            RoomTypeId = roomCreateDto.RoomTypeId,
            Img = imageUrl
        };
        await _roomService.CreateRoom(room, imageFile);
        return CreatedAtAction(nameof(Get), new { room });
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Patch(string id, [FromForm] RoomEditDto roomEditDto, IFormFile? imageFile)
    {
        var room = await _roomService.GetRoomById(id);
        if (room == null)
        {
            return NotFound("Room not found");
        }
        if (!string.IsNullOrEmpty(roomEditDto.NameRoom))
            room.NameRoom = roomEditDto.NameRoom;
        if (roomEditDto.Floor > 0)
            room.Floor = roomEditDto.Floor;
        if (roomEditDto.Status.HasValue)
            room.Status = roomEditDto.Status.Value;
        if (!string.IsNullOrEmpty(roomEditDto.RoomTypeId))
            room.RoomTypeId = roomEditDto.RoomTypeId;
        if (imageFile != null)
        {
            try
            {
                if (!string.IsNullOrEmpty(room.Img))
                {
                    string dropboxPath = await _dropboxService.GetDropboxPathFromUrl(room.Img);
                    if (!string.IsNullOrEmpty(dropboxPath))
                    {
                        await _dropboxService.DeleteFileAsync(dropboxPath);
                    }
                }

                string newImageUrl = await _dropboxService.UploadFileToRoomAsync(imageFile.OpenReadStream(), imageFile.FileName);
                room.Img = newImageUrl;
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update image on Dropbox: {ex.Message}");
            }
        }
        await _roomService.UpdateRoom(id, room);
        return Ok("Discount updated successfully");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id)
    {
        var room = await _roomService.GetRoomById(id);
        if (room == null)
        {
            return NotFound("Room not found");
        }

        if (!string.IsNullOrEmpty(room.Img))
        {
            try
            {

                string dropboxPath = await _dropboxService.GetDropboxPathFromUrl(room.Img);


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

        await _roomService.DeleteRoom(id);
        return Ok("Room deleted successfully");
    }
}