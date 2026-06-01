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

    Task<IReadOnlyList<ExerciseChapterProgress>> GetExerciseProgressAsync(
        string userSub,
        IReadOnlyList<(string Slug, int TotalParts)> exercises,
        CancellationToken cancellationToken = default);
}

public sealed class ExerciseProgressService(Gf2LearnDbContext db) : IExerciseProgressService
{
    private const int MaxAnswerLength = 16_000;

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

        var existing = await db.ExerciseAnswers
            .FirstOrDefaultAsync(
                a => a.UserSub == userSub
                     && a.ContentSlug == contentSlug
                     && a.PartIndex == partIndex,
                cancellationToken);

        if (existing is null)
        {
            db.ExerciseAnswers.Add(new ExerciseAnswer
            {
                UserSub = userSub,
                ContentSlug = contentSlug,
                PartIndex = partIndex,
                AnswerText = text,
                CompletedAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            existing.AnswerText = text;
            existing.CompletedAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExercisePartAnswerDto>> GetAnswersAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default)
    {
        return await db.ExerciseAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub && a.ContentSlug == contentSlug)
            .OrderBy(a => a.PartIndex)
            .Select(a => new ExercisePartAnswerDto(a.PartIndex, a.AnswerText, a.CompletedAt))
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
        var completedCounts = await db.ExerciseAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub && slugs.Contains(a.ContentSlug))
            .GroupBy(a => a.ContentSlug)
            .Select(g => new { Slug = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Slug, x => x.Count, cancellationToken);

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
