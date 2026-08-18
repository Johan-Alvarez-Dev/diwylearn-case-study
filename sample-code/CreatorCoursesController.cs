using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiwyLearn.PublicSample;

public sealed record CreateCourseRequest(string Title, string Slug);
public sealed record CreatedCourseResponse(Guid Id, string Title, string Slug);

[ApiController]
[Authorize(Roles = "Creator,Admin")]
[Route("api/creator/courses")]
public sealed class CreatorCoursesController(LearningDbContext dbContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreatedCourseResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatedCourseResponse>> Create(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var title = request.Title.Trim();
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (title.Length is < 3 or > 160 || slug.Length is < 3 or > 120)
            return BadRequest("Title or slug length is invalid.");

        if (await dbContext.Courses.AnyAsync(course => course.Slug == slug, cancellationToken))
            return Conflict("The slug is already in use.");

        var course = new PublicCourse
        {
            Title = title,
            Slug = slug,
            IsPublished = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CreatedCourseResponse(course.Id, course.Title, course.Slug);
        return CreatedAtAction(nameof(Create), new { id = course.Id }, response);
    }
}
