namespace DiwyLearn.PublicSample;

[Flags]
public enum UserRole { None = 0, Student = 1, Creator = 2, Admin = 4 }
public enum CourseAction { View, Edit, ReviewSubmissions }
public enum AccessOutcome { Allow, Deny }

public sealed record CourseAccessContext(
    UserRole Roles,
    CourseAction Action,
    bool IsOwner,
    bool IsPublished,
    bool IsEnrolled);

public sealed record AccessDecision(AccessOutcome Outcome, string Reason)
{
    public bool IsAllowed => Outcome is AccessOutcome.Allow;
}

public static class CourseAccessPolicy
{
    public static AccessDecision Evaluate(CourseAccessContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Roles.HasFlag(UserRole.Admin))
            return Allow("Administrator policy.");

        return context.Action switch
        {
            CourseAction.Edit when context.Roles.HasFlag(UserRole.Creator) && context.IsOwner
                => Allow("Creator owns the course."),
            CourseAction.ReviewSubmissions when context.Roles.HasFlag(UserRole.Creator) && context.IsOwner
                => Allow("Creator owns the course."),
            CourseAction.View when context.IsPublished
                => Allow("Course is published."),
            CourseAction.View when context.IsOwner
                => Allow("Owner can preview a draft."),
            CourseAction.View when context.IsEnrolled
                => Allow("Enrollment grants access."),
            _ => Deny("Role, ownership, publication, or enrollment requirement was not met.")
        };
    }

    private static AccessDecision Allow(string reason) => new(AccessOutcome.Allow, reason);
    private static AccessDecision Deny(string reason) => new(AccessOutcome.Deny, reason);
}
