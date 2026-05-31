namespace GF2Learn.Web.Models;

public sealed record SaveKnowledgeCheckAnswerRequest(
    string ContentSlug,
    int QuestionIndex,
    int SelectedIndex,
    bool IsCorrect);

public sealed record KnowledgeCheckAnswerDto(
    int QuestionIndex,
    int SelectedIndex,
    bool IsCorrect);

public sealed record CurriculumChapterProgress(
    string ContentSlug,
    int TotalQuestions,
    int AnsweredQuestions,
    bool IsComplete);
