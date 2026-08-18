using System.Net;
using System.Net.Http.Json;
using DiwyLearn.PublicSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DiwyLearn.PublicSample.Tests;

public sealed class PublicApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(), $"diwylearn-public-tests-{Guid.NewGuid()}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:LearningDb"] = $"Data Source={_databasePath}"
            }));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(_databasePath))
            File.Delete(_databasePath);
    }
}

public sealed class PublicApiTests(PublicApiFactory factory) : IClassFixture<PublicApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Catalog_ReturnsSeededPublishedCourses()
    {
        var response = await _client.GetAsync("/api/public/courses");

        response.EnsureSuccessStatusCode();
        var courses = await response.Content.ReadFromJsonAsync<List<CourseSummary>>();
        Assert.NotNull(courses);
        Assert.Equal(2, courses.Count);
        Assert.All(courses, course => Assert.True(course.ActiveEnrollments > 0));
    }

    [Fact]
    public async Task CreatorEndpoint_RejectsAnonymousRequests()
    {
        var request = new CreateCourseRequest("Secure APIs", "secure-apis");

        var response = await _client.PostAsJsonAsync("/api/creator/courses", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
