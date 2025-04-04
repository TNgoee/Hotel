using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewRoomController : ControllerBase
{
    private readonly IReviewRoomService _reviewRoomService;
    public ReviewRoomController(IReviewRoomService reviewRoomService)
    {
        _reviewRoomService = reviewRoomService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewRoom>>> Get()
    {
        var reviews = await _reviewRoomService.GetAllReviewRoom();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReviewRoom>> GetById(string id)
    {
        var review = await _reviewRoomService.GetReviewRoomById(id);
        if (review == null)
        {
            return NotFound("Review not found");
        }
        return Ok(review);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] ReviewRoomCreateDto reviewRoomCreateDto)
    {
        if (reviewRoomCreateDto == null)
        {
            return BadRequest("Invalid data");
        }
        var review = new ReviewRoom
        {
            CustomId = reviewRoomCreateDto.CustomId,
            RoomId = reviewRoomCreateDto.RoomId,
            Rating = reviewRoomCreateDto.Rating,
            Comment = reviewRoomCreateDto.Comment,
            CreatedAt = DateTime.UtcNow
        };
        await _reviewRoomService.CreateReviewRoom(review);
        return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ReviewRoom>> Patch(string id, [FromForm] ReviewRoomEditDto reviewRoomEditDto)
    {
        if (string.IsNullOrWhiteSpace(id) || reviewRoomEditDto == null)
        {
            return BadRequest("Invalid data");
        }
        var review = await _reviewRoomService.GetReviewRoomById(id);
        if (review == null)
        {
            return NotFound("Review not found");
        }

        if (reviewRoomEditDto.Rating.HasValue)
            review.Rating = reviewRoomEditDto.Rating.Value;


        if (!string.IsNullOrEmpty(reviewRoomEditDto.Comment))
            review.Comment = reviewRoomEditDto.Comment;

        var isUpdated = await _reviewRoomService.UpdateReviewRoom(id, review);
        if (!isUpdated)
        {
            return NotFound("Failed to update review");
        }
        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var isDeleted = await _reviewRoomService.DeleteReviewRoom(id);
        if (!isDeleted)
        {
            return NotFound("Review not found");
        }
        return NoContent();
    }
}
