using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IExerciseProgressService
{
    Task<ExercisePartVersionDto> SavePartAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        string? answerText,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExercisePartAnswerDto>> GetAnswersAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExercisePartVersionDto>> GetPartVersionsAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExerciseChapterProgress>> GetExerciseProgressAsync(
        string userSub,
        IReadOnlyList<(string Slug, int TotalParts)> exercises,
        CancellationToken cancellationToken = default);

    Task SaveVerificationAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        bool isSolved,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExercisePartVerificationDto>> GetVerificationsAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default);
}

public sealed class ExerciseProgressService(Gf2LearnDbContext db) : IExerciseProgressService
{
    private const int MaxAnswerLength = 16_000;
    private const int MaxVersionsPerPart = 3;

    public async Task<ExercisePartVersionDto> SavePartAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        string? answerText,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(answerText))
            throw new InvalidOperationException("Koden er tom — skriv noget før du gemmer.");

        var text = answerText.Length > MaxAnswerLength
            ? answerText[..MaxAnswerLength]
            : answerText.Trim();

        var versions = await db.ExerciseAnswers
            .Where(a => a.UserSub == userSub
                        && a.ContentSlug == contentSlug
                        && a.PartIndex == partIndex)
            .OrderBy(a => a.CompletedAt)
            .ToListAsync(cancellationToken);

        var latest = versions.MaxBy(v => v.CompletedAt);
        if (latest is not null && string.Equals(latest.AnswerText, text, StringComparison.Ordinal))
            return new ExercisePartVersionDto(latest.Id, latest.AnswerText, latest.CompletedAt);

        if (versions.Count >= MaxVersionsPerPart)
            db.ExerciseAnswers.Remove(versions[0]);

        var savedAt = DateTimeOffset.UtcNow;
        var row = new ExerciseAnswer
        {
            UserSub = userSub,
            ContentSlug = contentSlug,
            PartIndex = partIndex,
            AnswerText = text,
            CompletedAt = savedAt
        };

        db.ExerciseAnswers.Add(row);
        await db.SaveChangesAsync(cancellationToken);

        return new ExercisePartVersionDto(row.Id, row.AnswerText, savedAt);
    }

    public async Task<IReadOnlyList<ExercisePartAnswerDto>> GetAnswersAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default)
    {
        var rows = await db.ExerciseAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub && a.ContentSlug == contentSlug)
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(a => a.PartIndex)
            .Select(g =>
            {
                var latest = g.OrderByDescending(x => x.CompletedAt).First();
                return new ExercisePartAnswerDto(latest.PartIndex, latest.AnswerText, latest.CompletedAt);
            })
            .OrderBy(a => a.PartIndex)
            .ToList();
    }

    public async Task<IReadOnlyList<ExercisePartVersionDto>> GetPartVersionsAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        CancellationToken cancellationToken = default)
    {
        return await db.ExerciseAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub
                        && a.ContentSlug == contentSlug
                        && a.PartIndex == partIndex)
            .OrderByDescending(a => a.CompletedAt)
            .Take(MaxVersionsPerPart)
            .Select(a => new ExercisePartVersionDto(a.Id, a.AnswerText, a.CompletedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExerciseChapterProgress>> GetExerciseProgressAsync(
        string userSub,
        IReadOnlyList<(string Slug, int TotalParts)> exercises,
        CancellationToken cancellationToken = default)
    {
        var withParts = exercises.Where(e => e.TotalParts > 0).ToList();
        if (withParts.Count == 0)
            return [];

        var slugs = withParts.Select(e => e.Slug).ToList();
        var verifiedParts = await db.ExercisePartVerifications
            .AsNoTracking()
            .Where(v => v.UserSub == userSub
                        && slugs.Contains(v.ContentSlug)
                        && v.IsSolved)
            .Select(v => new { v.ContentSlug, v.PartIndex })
            .ToListAsync(cancellationToken);

        var completedCounts = verifiedParts
            .GroupBy(x => x.ContentSlug)
            .ToDictionary(g => g.Key, g => g.Count());

        return withParts
            .Select(e =>
            {
                var completed = completedCounts.GetValueOrDefault(e.Slug, 0);
                return new ExerciseChapterProgress(
                    e.Slug,
                    e.TotalParts,
                    completed,
                    completed >= e.TotalParts);
            })
            .ToList();
    }

    public async Task SaveVerificationAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        bool isSolved,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.ExercisePartVerifications
            .FirstOrDefaultAsync(
                v => v.UserSub == userSub
                     && v.ContentSlug == contentSlug
                     && v.PartIndex == partIndex,
                cancellationToken);

        var verifiedAt = DateTimeOffset.UtcNow;
        if (existing is null)
        {
            db.ExercisePartVerifications.Add(new ExercisePartVerification
            {
                UserSub = userSub,
                ContentSlug = contentSlug,
                PartIndex = partIndex,
                IsSolved = isSolved,
                VerifiedAt = verifiedAt
            });
        }
        else
        {
            existing.IsSolved = isSolved;
            existing.VerifiedAt = verifiedAt;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExercisePartVerificationDto>> GetVerificationsAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default)
    {
        return await db.ExercisePartVerifications
            .AsNoTracking()
            .Where(v => v.UserSub == userSub && v.ContentSlug == contentSlug)
            .OrderBy(v => v.PartIndex)
            .Select(v => new ExercisePartVerificationDto(v.PartIndex, v.IsSolved, v.VerifiedAt))
            .ToListAsync(cancellationToken);
    }
}
