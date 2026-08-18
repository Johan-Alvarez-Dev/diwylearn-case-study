using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DiwyLearn.PublicSample;

[ApiController]
[Route("api/public/courses")]
public sealed class CoursesController(CourseCatalogQuery catalogQuery) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CourseSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CourseSummary>>> GetPublished(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var courses = await catalogQuery.ExecuteAsync(page, pageSize, cancellationToken);
        return Ok(courses);
    }
}
