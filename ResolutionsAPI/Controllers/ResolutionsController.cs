using Microsoft.AspNetCore.Mvc;
using ResolutionsApi.Data;
using ResolutionsApi.Helpers;
using ResolutionsApi.Models;

namespace ResolutionsApi.Controllers;

[ApiController]
[Route("api/resolutions")]
public class ResolutionsController : ControllerBase
{
    // ===================== GET ALL =====================
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? isDone, [FromQuery] string? title)
    {
        var items = ResolutionStore.Resolutions.AsEnumerable();

        // Filter by isDone
        if (!string.IsNullOrEmpty(isDone))
        {
            if (!bool.TryParse(isDone, out bool done))
            {
                return BadRequest(ErrorResponse.BadRequest(
                    "Validation failed.",
                    "isDone must be true or false"
                ));
            }
            items = items.Where(r => r.IsDone == done);
        }

        // Search by title
        if (!string.IsNullOrWhiteSpace(title))
        {
            items = items.Where(r =>
                r.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(new
        {
            items = items.Select(r => new
            {
                id = r.Id,
                title = r.Title,
                isDone = r.IsDone
            })
        });
    }

    // ===================== GET BY ID =====================
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id <= 0)
            return BadRequest(ErrorResponse.BadRequest("Validation failed.", "id must be greater than 0"));

        var resolution = ResolutionStore.Resolutions.FirstOrDefault(r => r.Id == id);
        if (resolution == null)
            return NotFound(ErrorResponse.NotFound("Resolution not found", $"id: {id}"));

        return Ok(new
        {
            id = resolution.Id,
            title = resolution.Title,
            isDone = resolution.IsDone,
            createdAt = resolution.CreatedAt
        });
    }

    // ===================== POST =====================
    [HttpPost]
    public IActionResult Create([FromBody] Resolution input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.Title))
        {
            return BadRequest(ErrorResponse.BadRequest(
                "Validation failed.",
                "title is required"
            ));
        }

        var resolution = new Resolution
        {
            Id = ResolutionStore.NextId,
            Title = input.Title.Trim(),
            IsDone = false,
            CreatedAt = DateTime.UtcNow
        };

        ResolutionStore.Resolutions.Add(resolution);

        return CreatedAtAction(nameof(GetById), new { id = resolution.Id }, resolution);
    }

    // ===================== PUT =====================
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Resolution input)
    {
        // Validation
        if (id <= 0)
            return BadRequest(ErrorResponse.BadRequest("Validation failed.", "route id must be greater than 0"));

        if (input == null || string.IsNullOrWhiteSpace(input.Title))
            return BadRequest(ErrorResponse.BadRequest("Validation failed.", "title is required"));

        if (id != input.Id)
            return BadRequest(ErrorResponse.BadRequest(
                "Route id does not match body id.",
                $"route id: {id}",
                $"body id: {input.Id}"
            ));

        var resolution = ResolutionStore.Resolutions.FirstOrDefault(r => r.Id == id);
        if (resolution == null)
            return NotFound(ErrorResponse.NotFound("Resolution not found", $"id: {id}"));

        // Update
        resolution.Title = input.Title.Trim();
        resolution.IsDone = input.IsDone;
        resolution.UpdatedAt = DateTime.UtcNow;

        return Ok(resolution);
    }

    // ===================== DELETE =====================
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
            return BadRequest(ErrorResponse.BadRequest("Validation failed.", "id must be greater than 0"));

        var resolution = ResolutionStore.Resolutions.FirstOrDefault(r => r.Id == id);
        if (resolution == null)
            return NotFound(ErrorResponse.NotFound("Resolution not found", $"id: {id}"));

        ResolutionStore.Resolutions.Remove(resolution);
        return NoContent(); // 204 No Content
    }
}
