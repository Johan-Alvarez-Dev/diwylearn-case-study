using DiwyLearn.PublicSample;
using Xunit;

namespace DiwyLearn.PublicSample.Tests;

public sealed class CourseAccessPolicyTests
{
    [Fact]
    public void Creator_can_edit_owned_course()
    {
        var context = new CourseAccessContext(UserRole.Creator, CourseAction.Edit, true, false, false);
        Assert.True(CourseAccessPolicy.Evaluate(context).IsAllowed);
    }

    [Fact]
    public void Creator_cannot_edit_another_creators_course()
    {
        var context = new CourseAccessContext(UserRole.Creator, CourseAction.Edit, false, true, false);
        Assert.False(CourseAccessPolicy.Evaluate(context).IsAllowed);
    }

    [Fact]
    public void Student_can_view_published_course()
    {
        var context = new CourseAccessContext(UserRole.Student, CourseAction.View, false, true, false);
        Assert.True(CourseAccessPolicy.Evaluate(context).IsAllowed);
    }

    [Fact]
    public void Admin_bypasses_course_level_rules()
    {
        var context = new CourseAccessContext(UserRole.Admin, CourseAction.ReviewSubmissions, false, false, false);
        Assert.True(CourseAccessPolicy.Evaluate(context).IsAllowed);
    }
}
