using DiwyLearn.PublicSample;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DiwyLearn.PublicSample.Tests;

public sealed class CourseCatalogQueryTests
{
    [Fact]
    public async Task Projects_only_published_courses_with_active_enrollment_count()
    {
        await using var db = CreateDb();
        var published = new PublicCourse { Title = "Architecture", Slug = "architecture", IsPublished = true };
        published.Enrollments.Add(new PublicEnrollment { CourseId = published.Id, IsActive = true });
        published.Enrollments.Add(new PublicEnrollment { CourseId = published.Id, IsActive = false });
        db.Courses.AddRange(published, new PublicCourse { Title = "Draft", Slug = "draft", IsPublished = false });
        await db.SaveChangesAsync();

        var result = await new CourseCatalogQuery(db).ExecuteAsync(1, 20);

        var course = Assert.Single(result);
        Assert.Equal("Architecture", course.Title);
        Assert.Equal(1, course.ActiveEnrollments);
    }

    [Fact]
    public async Task Mvc_controller_returns_explicit_ok_result()
    {
        await using var db = CreateDb();
        var controller = new CoursesController(new CourseCatalogQuery(db));

        var action = await controller.GetPublished();

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.IsAssignableFrom<IReadOnlyList<CourseSummary>>(ok.Value);
    }

    private static LearningDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<LearningDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new LearningDbContext(options);
    }
}
