using Microsoft.EntityFrameworkCore;

namespace DiwyLearn.PublicSample;

public sealed class PublicCourse
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public List<PublicEnrollment> Enrollments { get; init; } = [];
}

public sealed class PublicEnrollment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CourseId { get; init; }
    public bool IsActive { get; set; }
}

public sealed class LearningDbContext(DbContextOptions<LearningDbContext> options) : DbContext(options)
{
    public DbSet<PublicCourse> Courses => Set<PublicCourse>();
    public DbSet<PublicEnrollment> Enrollments => Set<PublicEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicCourse>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Title).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(120).IsRequired();
            entity.HasMany(x => x.Enrollments).WithOne().HasForeignKey(x => x.CourseId);
        });
        modelBuilder.Entity<PublicEnrollment>().HasKey(x => x.Id);
    }
}

public sealed record CourseSummary(Guid Id, string Title, string Slug, int ActiveEnrollments);

public sealed class CourseCatalogQuery(LearningDbContext dbContext)
{
    public async Task<IReadOnlyList<CourseSummary>> ExecuteAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        return await dbContext.Courses
            .AsNoTracking()
            .Where(course => course.IsPublished)
            .OrderBy(course => course.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(course => new CourseSummary(
                course.Id,
                course.Title,
                course.Slug,
                course.Enrollments.Count(enrollment => enrollment.IsActive)))
            .ToListAsync(cancellationToken);
    }
}
