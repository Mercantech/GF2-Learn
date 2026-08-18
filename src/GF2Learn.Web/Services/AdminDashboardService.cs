using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IAdminDashboardService
{
    Task<AdminDashboardDto> GetDashboardAsync(
        AdminDashboardQuery query,
        CancellationToken cancellationToken = default);

    Task<AdminStudentDetailDto?> GetStudentAsync(
        Guid userId,
        int periodDays = 30,
        CancellationToken cancellationToken = default);

    Task SetNicknameAsync(
        Guid userId,
        string? nickname,
        CancellationToken cancellationToken = default);
}

public sealed class AdminDashboardService(
    IDbContextFactory<Gf2LearnDbContext> dbFactory,
    ContentService content,
    ExerciseCatalog exerciseCatalog,
    KnowledgeCheckCatalog knowledgeCheckCatalog) : IAdminDashboardService
{
    private const int InactiveAfterDays = 7;

    public async Task<AdminDashboardDto> GetDashboardAsync(
        AdminDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var inactiveCutoff = now.AddDays(-InactiveAfterDays);
        var usersQuery = db.AppUsers
            .AsNoTracking()
            .Where(user => !user.IsEducator);

        if (query.GroupId is { } groupId)
        {
            usersQuery = usersQuery.Where(user =>
                user.GroupMemberships.Any(member => member.GroupId == groupId));
        }

        var search = query.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            usersQuery = usersQuery.Where(user =>
                (user.AdminMetadata != null
                    && user.AdminMetadata.Nickname != null
                    && EF.Functions.ILike(user.AdminMetadata.Nickname, pattern))
                || EF.Functions.ILike(user.UserSub, pattern));
        }

        var students = await usersQuery
            .Include(user => user.AdminMetadata)
            .Include(user => user.GroupMemberships)
                .ThenInclude(member => member.Group)
            .ToListAsync(cancellationToken);

        var userIds = students.Select(user => user.Id).ToList();
        var userSubs = students.Select(user => user.UserSub).ToList();
        var activityQuery = db.PageActivityDaily
            .AsNoTracking()
            .Where(activity => userIds.Contains(activity.UserId));
        if (query.PeriodDays > 0)
        {
            var cutoffDate = DateOnly.FromDateTime(now.UtcDateTime.AddDays(-(query.PeriodDays - 1)));
            activityQuery = activityQuery.Where(activity => activity.ActivityDate >= cutoffDate);
        }

        var activeSeconds = await activityQuery
            .GroupBy(activity => activity.UserId)
            .Select(group => new { UserId = group.Key, Seconds = group.Sum(item => item.ActiveSeconds) })
            .ToDictionaryAsync(item => item.UserId, item => item.Seconds, cancellationToken);

        var exercisePartsBySlug = content.GetAll()
            .Where(item => item.Section == ContentSectionType.Exercises)
            .ToDictionary(
                item => item.Slug,
                item => exerciseCatalog.CountParts(item.Body),
                StringComparer.Ordinal);
        var totalParts = exercisePartsBySlug.Values.Sum();

        // Historic clients could submit arbitrary slugs/part indexes. Only count
        // solved parts that still exist in the current exercise catalogue so the
        // overview and individual detail use the same completion definition.
        var solvedParts = userSubs.Count == 0
            ? []
            : await db.ExercisePartVerifications
                .AsNoTracking()
                .Where(item => userSubs.Contains(item.UserSub) && item.IsSolved)
                .Select(item => new { item.UserSub, item.ContentSlug, item.PartIndex })
                .ToListAsync(cancellationToken);
        var completedParts = solvedParts
            .Where(item => exercisePartsBySlug.TryGetValue(item.ContentSlug, out var partCount)
                           && item.PartIndex >= 0
                           && item.PartIndex < partCount)
            .GroupBy(item => item.UserSub, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        var rows = students
            .Select(user =>
            {
                var isInactive = user.LastActivityAt is null || user.LastActivityAt < inactiveCutoff;
                return new AdminStudentRowDto(
                    user.Id,
                    StudentLabel(user.UserSub, user.Id),
                    user.AdminMetadata?.Nickname,
                    user.GroupMemberships
                        .Where(member => !member.Group.IsArchived)
                        .Select(member => member.Group.Name)
                        .OrderBy(name => name)
                        .ToList(),
                    user.LastActivityAt,
                    activeSeconds.GetValueOrDefault(user.Id),
                    completedParts.GetValueOrDefault(user.UserSub),
                    totalParts,
                    isInactive);
            })
            .Where(row => (query.Status ?? "all").ToLowerInvariant() switch
            {
                "inactive" => row.IsInactive,
                "active" => !row.IsInactive,
                _ => true
            })
            .OrderByDescending(row => row.IsInactive)
            .ThenBy(row => row.IsInactive ? row.LastActivityAt : null)
            .ThenBy(row => row.Nickname ?? row.StudentLabel, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var visibleUserIds = rows.Select(row => row.UserId).ToList();
        var popularContent = await BuildPopularContentAsync(
            activityQuery.Where(item => visibleUserIds.Contains(item.UserId)),
            cancellationToken);

        var groups = await db.LearningGroups
            .AsNoTracking()
            .Where(group => !group.IsArchived)
            .OrderBy(group => group.Name)
            .Select(group => new AdminGroupOptionDto(
                group.Id,
                group.Name,
                group.Members.Count(member => !member.User.IsEducator)))
            .ToListAsync(cancellationToken);

        return new AdminDashboardDto(
            rows.Count,
            rows.Count(row => !row.IsInactive),
            rows.Count(row => row.IsInactive),
            rows.Sum(row => row.ActiveSeconds),
            rows.Sum(row => row.CompletedParts),
            rows,
            popularContent,
            groups,
            now);
    }

    public async Task<AdminStudentDetailDto?> GetStudentAsync(
        Guid userId,
        int periodDays = 30,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var user = await db.AppUsers
            .AsNoTracking()
            .Include(item => item.AdminMetadata)
            .Include(item => item.GroupMemberships)
                .ThenInclude(member => member.Group)
            .FirstOrDefaultAsync(item => item.Id == userId && !item.IsEducator, cancellationToken);
        if (user is null)
            return null;

        var allContent = content.GetAll()
            .Where(item => item.Section is ContentSectionType.Curriculum or ContentSectionType.Exercises)
            .OrderBy(item => item.Section)
            .ThenBy(item => item.Order)
            .ToList();
        var exercisePartsBySlug = allContent
            .Where(item => item.Section == ContentSectionType.Exercises)
            .ToDictionary(
                item => item.Slug,
                item => exerciseCatalog.CountParts(item.Body),
                StringComparer.Ordinal);
        var questionsBySlug = allContent
            .Where(item => item.Section == ContentSectionType.Curriculum)
            .ToDictionary(
                item => item.Slug,
                item => knowledgeCheckCatalog.CountQuestions(item.Body),
                StringComparer.Ordinal);

        var now = DateTimeOffset.UtcNow;
        var activitiesQuery = db.PageActivityDaily
            .AsNoTracking()
            .Where(item => item.UserId == userId);
        if (periodDays > 0)
        {
            var cutoffDate = DateOnly.FromDateTime(now.UtcDateTime.AddDays(-(periodDays - 1)));
            activitiesQuery = activitiesQuery.Where(item => item.ActivityDate >= cutoffDate);
        }

        var activities = await activitiesQuery
            .GroupBy(item => new { item.ContentType, item.ContentSlug })
            .Select(group => new
            {
                group.Key.ContentType,
                group.Key.ContentSlug,
                Seconds = group.Sum(item => item.ActiveSeconds),
                Visits = group.Sum(item => item.VisitCount),
                LastSeenAt = group.Max(item => item.LastSeenAt)
            })
            .ToListAsync(cancellationToken);
        var activityByContent = activities.ToDictionary(
            item => (item.ContentType, item.ContentSlug),
            item => (item.Seconds, item.Visits, (DateTimeOffset?)item.LastSeenAt));

        var verifiedParts = await db.ExercisePartVerifications
            .AsNoTracking()
            .Where(item => item.UserSub == user.UserSub && item.IsSolved)
            .Select(item => new { item.ContentSlug, item.PartIndex })
            .ToListAsync(cancellationToken);
        var verified = verifiedParts
            .Where(item => exercisePartsBySlug.TryGetValue(item.ContentSlug, out var partCount)
                           && item.PartIndex >= 0
                           && item.PartIndex < partCount)
            .GroupBy(item => item.ContentSlug, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        var answeredQuestionRows = await db.KnowledgeCheckAnswers
            .AsNoTracking()
            .Where(item => item.UserSub == user.UserSub)
            .Select(item => new { item.ContentSlug, item.QuestionIndex })
            .ToListAsync(cancellationToken);
        var answered = answeredQuestionRows
            .Where(item => questionsBySlug.TryGetValue(item.ContentSlug, out var questionCount)
                           && item.QuestionIndex >= 0
                           && item.QuestionIndex < questionCount)
            .GroupBy(item => item.ContentSlug, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        var rows = new List<AdminStudentContentRowDto>(allContent.Count);
        foreach (var item in allContent)
        {
            var type = item.Section == ContentSectionType.Exercises ? "exercise" : "curriculum";
            var total = item.Section == ContentSectionType.Exercises
                ? exercisePartsBySlug[item.Slug]
                : questionsBySlug[item.Slug];
            var completed = item.Section == ContentSectionType.Exercises
                ? verified.GetValueOrDefault(item.Slug)
                : answered.GetValueOrDefault(item.Slug);
            var activity = activityByContent.GetValueOrDefault((type, item.Slug));

            rows.Add(new AdminStudentContentRowDto(
                type,
                item.Slug,
                item.Title,
                item.Category,
                Math.Min(completed, total),
                total,
                activity.Seconds,
                activity.Visits,
                activity.Item3));
        }

        var totalExerciseParts = rows
            .Where(row => row.ContentType == "exercise")
            .Sum(row => row.TotalItems);
        var completedExerciseParts = rows
            .Where(row => row.ContentType == "exercise")
            .Sum(row => row.CompletedItems);
        var totalQuestions = rows
            .Where(row => row.ContentType == "curriculum")
            .Sum(row => row.TotalItems);
        var answeredQuestions = rows
            .Where(row => row.ContentType == "curriculum")
            .Sum(row => row.CompletedItems);

        return new AdminStudentDetailDto(
            user.Id,
            StudentLabel(user.UserSub, user.Id),
            user.AdminMetadata?.Nickname,
            user.GroupMemberships
                .Where(member => !member.Group.IsArchived)
                .Select(member => member.Group.Name)
                .OrderBy(name => name)
                .ToList(),
            user.FirstSeenAt,
            user.LastActivityAt,
            rows.Sum(row => row.ActiveSeconds),
            completedExerciseParts,
            totalExerciseParts,
            answeredQuestions,
            totalQuestions,
            rows);
    }

    public async Task SetNicknameAsync(
        Guid userId,
        string? nickname,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        if (!await db.AppUsers.AnyAsync(
                user => user.Id == userId && !user.IsEducator,
                cancellationToken))
        {
            throw new InvalidOperationException("Eleven findes ikke.");
        }

        var cleaned = nickname?.Trim();
        if (cleaned?.Length > 200)
            cleaned = cleaned[..200];
        if (string.IsNullOrWhiteSpace(cleaned))
            cleaned = null;

        var metadata = await db.LearnerAdminMetadata
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (metadata is null)
        {
            db.LearnerAdminMetadata.Add(new LearnerAdminMetadata
            {
                UserId = userId,
                Nickname = cleaned
            });
        }
        else
        {
            metadata.Nickname = cleaned;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<AdminContentUsageDto>> BuildPopularContentAsync(
        IQueryable<PageActivityDaily> activityQuery,
        CancellationToken cancellationToken)
    {
        var usage = await activityQuery
            .GroupBy(item => new { item.ContentType, item.ContentSlug })
            .Select(group => new
            {
                group.Key.ContentType,
                group.Key.ContentSlug,
                Seconds = group.Sum(item => item.ActiveSeconds),
                Visits = group.Sum(item => item.VisitCount),
                Students = group.Select(item => item.UserId).Distinct().Count()
            })
            .OrderByDescending(item => item.Seconds)
            .ThenByDescending(item => item.Visits)
            .Take(6)
            .ToListAsync(cancellationToken);

        return usage.Select(item => new AdminContentUsageDto(
                item.ContentType,
                item.ContentSlug,
                ContentTitle(item.ContentType, item.ContentSlug),
                item.Seconds,
                item.Visits,
                item.Students))
            .ToList();
    }

    private string ContentTitle(string contentType, string slug) =>
        contentType == "exercise"
            ? content.GetExercise(slug)?.Title ?? slug
            : content.GetCurriculum(slug)?.Title ?? slug;

    private static string StudentLabel(string userSub, Guid id)
    {
        var normalized = userSub.Trim();
        var identifier = normalized.Length == 0
            ? id.ToString("N")[..8]
            : normalized[..Math.Min(normalized.Length, 8)];
        return $"Elev {identifier.ToUpperInvariant()}";
    }
}
