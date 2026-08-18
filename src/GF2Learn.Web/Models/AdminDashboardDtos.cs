namespace GF2Learn.Web.Models;

public sealed record AdminDashboardQuery(
    Guid? GroupId = null,
    string? Search = null,
    int PeriodDays = 7,
    string Status = "all");

public sealed record AdminGroupOptionDto(Guid Id, string Name, int StudentCount);

public sealed record AdminStudentRowDto(
    Guid UserId,
    string StudentLabel,
    string? Nickname,
    IReadOnlyList<string> Groups,
    DateTimeOffset? LastActivityAt,
    int ActiveSeconds,
    int CompletedParts,
    int TotalParts,
    bool IsInactive);

public sealed record AdminContentUsageDto(
    string ContentType,
    string ContentSlug,
    string Title,
    int ActiveSeconds,
    int VisitCount,
    int ActiveStudents);

public sealed record AdminDashboardDto(
    int TotalStudents,
    int ActiveStudents,
    int InactiveStudents,
    int ActiveSeconds,
    int CompletedParts,
    IReadOnlyList<AdminStudentRowDto> Students,
    IReadOnlyList<AdminContentUsageDto> PopularContent,
    IReadOnlyList<AdminGroupOptionDto> Groups,
    DateTimeOffset GeneratedAt);

public sealed record AdminStudentDetailDto(
    Guid UserId,
    string StudentLabel,
    string? Nickname,
    IReadOnlyList<string> Groups,
    DateTimeOffset FirstSeenAt,
    DateTimeOffset? LastActivityAt,
    int ActiveSeconds,
    int CompletedParts,
    int TotalParts,
    int AnsweredQuestions,
    int TotalQuestions,
    IReadOnlyList<AdminStudentContentRowDto> ContentRows);

public sealed record AdminStudentContentRowDto(
    string ContentType,
    string ContentSlug,
    string Title,
    string? Category,
    int CompletedItems,
    int TotalItems,
    int ActiveSeconds,
    int VisitCount,
    DateTimeOffset? LastSeenAt);

public sealed record LearningGroupSummaryDto(
    Guid Id,
    string Name,
    bool IsArchived,
    int StudentCount,
    DateTimeOffset CreatedAt);

public sealed record LearningGroupMemberDto(
    Guid UserId,
    string StudentLabel,
    string? Nickname,
    DateTimeOffset JoinedAt,
    string Source);

public sealed record AvailableStudentDto(
    Guid UserId,
    string StudentLabel,
    string? Nickname,
    bool IsMember);

public sealed record LearningGroupDetailDto(
    Guid Id,
    string Name,
    bool IsArchived,
    IReadOnlyList<LearningGroupMemberDto> Members,
    IReadOnlyList<AvailableStudentDto> AvailableStudents);

public sealed record GeneratedGroupAccessDto(
    string Value,
    string RelativeJoinUrl,
    DateTimeOffset? ExpiresAt,
    string Kind);

public sealed record JoinGroupResult(bool Success, string Message, string? GroupName = null);

public sealed record PageActivityHeartbeatRequest(
    Guid SessionId,
    string ContentType,
    string ContentSlug,
    int ActiveSeconds,
    DateTimeOffset StartedAt);
