using Microsoft.EntityFrameworkCore;

namespace DiwyLearn.PublicSample;

public static class SampleData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        if (await dbContext.Courses.AnyAsync())
            return;

        var architecture = new PublicCourse
        {
            Title = "Practical Software Architecture",
            Slug = "practical-software-architecture",
            IsPublished = true,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            Enrollments =
            [
                new PublicEnrollment { IsActive = true },
                new PublicEnrollment { IsActive = true },
                new PublicEnrollment { IsActive = false }
            ]
        };

        var testing = new PublicCourse
        {
            Title = "Reliable .NET Testing",
            Slug = "reliable-dotnet-testing",
            IsPublished = true,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-14),
            Enrollments = [new PublicEnrollment { IsActive = true }]
        };

        dbContext.Courses.AddRange(architecture, testing);
        await dbContext.SaveChangesAsync();
    }
}
