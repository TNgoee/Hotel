using KhachSan.Models;
using KhachSan.Services;
using Microsoft.AspNetCore.Mvc;

namespace KhachSan.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewServiceController : ControllerBase
{
    private readonly IReviewServiceService _reviewServiceService;
    public ReviewServiceController(IReviewServiceService reviewServiceService)
    {
        _reviewServiceService = reviewServiceService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewService>>> Get()
    {
        var reviews = await _reviewServiceService.GetAllReviewService();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReviewService>> GetById(string id)
    {
        var review = await _reviewServiceService.GetReviewServiceById(id);
        if (review == null)
        {
            return NotFound("Review not found");
        }
        return Ok(review);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Post([FromForm] ReviewServiceCreateDto reviewServiceCreateDto)
    {
        if (reviewServiceCreateDto == null)
        {
            return BadRequest("Invalid data");
        }
        var review = new ReviewService
        {
            CustomId = reviewServiceCreateDto.CustomId,
            ServiceId = reviewServiceCreateDto.ServiceId,
            Rating = reviewServiceCreateDto.Rating,
            Comment = reviewServiceCreateDto.Comment,
            CreatedAt = DateTime.UtcNow
        };
        await _reviewServiceService.CreateReviewService(review);
        return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ReviewService>> Patch(string id, [FromForm] ReviewServiceEditDto reviewServiceEditDto)
    {
        if (string.IsNullOrWhiteSpace(id) || reviewServiceEditDto == null)
        {
            return BadRequest("Invalid data");
        }
        var review = await _reviewServiceService.GetReviewServiceById(id);
        if (review == null)
        {
            return NotFound("Review not found");
        }

        if (reviewServiceEditDto.Rating != null)
            review.Rating = reviewServiceEditDto.Rating.Value;


        if (!string.IsNullOrEmpty(reviewServiceEditDto.Comment))
            review.Comment = reviewServiceEditDto.Comment;

        var isUpdated = await _reviewServiceService.UpdateReviewService(id, review);
        if (!isUpdated)
        {
            return NotFound("Failed to update review");
        }
        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var isDeleted = await _reviewServiceService.DeleteReviewService(id);
        if (!isDeleted)
        {
            return NotFound("Review not found");
        }
        return NoContent();
    }
}
