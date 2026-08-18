namespace GF2Learn.Web.Models;

public sealed class AppUser
{
    public Guid Id { get; set; }
    public required string UserSub { get; set; }
    public bool IsEducator { get; set; }
    public bool IsSuperAdmin { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastLoginAt { get; set; }
    public DateTimeOffset? LastActivityAt { get; set; }

    public LearnerAdminMetadata? AdminMetadata { get; set; }
    public ICollection<LearningGroupMember> GroupMemberships { get; set; } = [];
    public ICollection<PageActivitySession> ActivitySessions { get; set; } = [];
    public ICollection<PageActivityDaily> DailyActivity { get; set; } = [];
    public PageActivityCreditGate? ActivityCreditGate { get; set; }
}

public sealed class LearnerAdminMetadata
{
    public Guid UserId { get; set; }
    public string? Nickname { get; set; }

    public AppUser User { get; set; } = null!;
}

public sealed class LearningGroup
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public required string CreatedBySub { get; set; }

    public ICollection<LearningGroupMember> Members { get; set; } = [];
    public ICollection<LearningGroupAccessToken> AccessTokens { get; set; } = [];
}

public sealed class LearningGroupMember
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public GroupMembershipSource Source { get; set; }
    public string? AddedBySub { get; set; }

    public LearningGroup Group { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}

public enum GroupMembershipSource
{
    Manual,
    Invite,
    Code
}

public sealed class LearningGroupAccessToken
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public GroupAccessTokenKind Kind { get; set; }
    public required string TokenHash { get; set; }
    public string? DisplaySuffix { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public int? MaxUses { get; set; }
    public int UseCount { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string CreatedBySub { get; set; }

    public LearningGroup Group { get; set; } = null!;
}

public enum GroupAccessTokenKind
{
    InviteLink,
    JoinCode
}

public sealed class PageActivitySession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string ContentType { get; set; }
    public required string ContentSlug { get; set; }
    public int ReportedActiveSeconds { get; set; }
    public bool VisitCredited { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset LastHeartbeatAt { get; set; }
    public int Version { get; set; }

    public AppUser User { get; set; } = null!;
}

public sealed class PageActivityDaily
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public required string ContentType { get; set; }
    public required string ContentSlug { get; set; }
    public DateOnly ActivityDate { get; set; }
    public int ActiveSeconds { get; set; }
    public int VisitCount { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public int Version { get; set; }

    public AppUser User { get; set; } = null!;
}

/// <summary>
/// Transient, server-owned accounting state. One shared bucket per learner limits
/// credited activity across every browser tab and client-generated session id.
/// </summary>
public sealed class PageActivityCreditGate
{
    public Guid UserId { get; set; }
    public int AvailableSeconds { get; set; }
    public DateTimeOffset LastRefillAt { get; set; }
    public int Version { get; set; }

    public AppUser User { get; set; } = null!;
}
