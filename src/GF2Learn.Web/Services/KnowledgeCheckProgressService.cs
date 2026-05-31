using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IKnowledgeCheckProgressService
{
    Task SaveAnswerAsync(
        string userSub,
        string contentSlug,
        int questionIndex,
        int selectedIndex,
        bool isCorrect,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<KnowledgeCheckAnswerDto>> GetAnswersAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CurriculumChapterProgress>> GetCurriculumProgressAsync(
        string userSub,
        IReadOnlyList<(string Slug, int TotalQuestions)> chapters,
        CancellationToken cancellationToken = default);
}

public sealed class KnowledgeCheckProgressService(Gf2LearnDbContext db) : IKnowledgeCheckProgressService
{
    public async Task SaveAnswerAsync(
        string userSub,
        string contentSlug,
        int questionIndex,
        int selectedIndex,
        bool isCorrect,
        CancellationToken cancellationToken = default)
    {
        var existing = await db.KnowledgeCheckAnswers
            .FirstOrDefaultAsync(
                a => a.UserSub == userSub
                     && a.ContentSlug == contentSlug
                     && a.QuestionIndex == questionIndex,
                cancellationToken);

        if (existing is null)
        {
            db.KnowledgeCheckAnswers.Add(new KnowledgeCheckAnswer
            {
                UserSub = userSub,
                ContentSlug = contentSlug,
                QuestionIndex = questionIndex,
                SelectedIndex = selectedIndex,
                IsCorrect = isCorrect,
                AnsweredAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            existing.SelectedIndex = selectedIndex;
            existing.IsCorrect = isCorrect;
            existing.AnsweredAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<KnowledgeCheckAnswerDto>> GetAnswersAsync(
        string userSub,
        string contentSlug,
        CancellationToken cancellationToken = default)
    {
        return await db.KnowledgeCheckAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub && a.ContentSlug == contentSlug)
            .OrderBy(a => a.QuestionIndex)
            .Select(a => new KnowledgeCheckAnswerDto(a.QuestionIndex, a.SelectedIndex, a.IsCorrect))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CurriculumChapterProgress>> GetCurriculumProgressAsync(
        string userSub,
        IReadOnlyList<(string Slug, int TotalQuestions)> chapters,
        CancellationToken cancellationToken = default)
    {
        var slugsWithQuestions = chapters
            .Where(c => c.TotalQuestions > 0)
            .Select(c => c.Slug)
            .ToList();

        if (slugsWithQuestions.Count == 0)
            return [];

        var answeredCounts = await db.KnowledgeCheckAnswers
            .AsNoTracking()
            .Where(a => a.UserSub == userSub && slugsWithQuestions.Contains(a.ContentSlug))
            .GroupBy(a => a.ContentSlug)
            .Select(g => new { Slug = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Slug, x => x.Count, cancellationToken);

        return chapters
            .Where(c => c.TotalQuestions > 0)
            .Select(c =>
            {
                var answered = answeredCounts.GetValueOrDefault(c.Slug, 0);
                return new CurriculumChapterProgress(
                    c.Slug,
                    c.TotalQuestions,
                    answered,
                    answered >= c.TotalQuestions);
            })
            .ToList();
    }
}
