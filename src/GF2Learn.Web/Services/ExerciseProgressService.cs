using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IExerciseProgressService
{
    Task SavePartAsync(
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
}

public sealed class ExerciseProgressService(Gf2LearnDbContext db) : IExerciseProgressService
{
    private const int MaxAnswerLength = 16_000;
    private const int MaxVersionsPerPart = 3;

    public async Task SavePartAsync(
        string userSub,
        string contentSlug,
        int partIndex,
        string? answerText,
        CancellationToken cancellationToken = default)
    {
        var text = string.IsNullOrWhiteSpace(answerText)
            ? null
            : answerText.Length > MaxAnswerLength
                ? answerText[..MaxAnswerLength]
                : answerText.Trim();

        var versions = await db.ExerciseAnswers
            .Where(a => a.UserSub == userSub
                        && a.ContentSlug == contentSlug
                        && a.PartIndex == partIndex)
            .OrderBy(a => a.CompletedAt)
            .ToListAsync(cancellationToken);

        if (versions.Count >= MaxVersionsPerPart)
            db.ExerciseAnswers.Remove(versions[0]);

        db.ExerciseAnswers.Add(new ExerciseAnswer
        {
            UserSub = userSub,
            ContentSlug = contentSlug,
            PartIndex = partIndex,
            AnswerText = text,
            CompletedAt = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
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
        var partKeys = await db.ExerciseAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub && slugs.Contains(a.ContentSlug))
            .Select(a => new { a.ContentSlug, a.PartIndex })
            .Distinct()
            .ToListAsync(cancellationToken);

        var completedCounts = partKeys
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
}
