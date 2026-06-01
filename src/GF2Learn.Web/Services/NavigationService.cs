using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed class NavigationService(ContentService content)
{
    public SectionNav GetSectionNav(ContentSectionType section)
    {
        var items = content.GetAll().Where(i => i.Section == section).ToList();
        return section switch
        {
            ContentSectionType.Curriculum => BuildCurriculumNav(items),
            ContentSectionType.Exercises => BuildExercisesNav(items),
            ContentSectionType.Projects => BuildProjectsNav(items),
            _ => new SectionNav { Section = section }
        };
    }

    private static SectionNav BuildCurriculumNav(List<ContentItem> items)
    {
        var csharp = items.Where(i => !i.Topics.Contains("git", StringComparer.OrdinalIgnoreCase)).OrderBy(i => i.Order).ToList();
        var git = items.Where(i => i.Topics.Contains("git", StringComparer.OrdinalIgnoreCase)).OrderBy(i => i.Order).ToList();
        var groups = new List<NavGroup>();
        if (csharp.Count > 0) groups.Add(new NavGroup { Title = "C#", Items = csharp });
        if (git.Count > 0) groups.Add(new NavGroup { Title = "Git", Items = git });
        if (groups.Count == 0) groups.Add(new NavGroup { Title = "Curriculum", Items = items.OrderBy(i => i.Order).ToList() });
        return new SectionNav { Section = ContentSectionType.Curriculum, Groups = groups };
    }

    private static SectionNav BuildExercisesNav(List<ContentItem> items)
    {
        var levels = new[] { ("begynder", "Beginner"), ("mellem", "Intermediate"), ("avanceret", "Advanced") };
        var groups = levels
            .Select(l => new NavGroup
            {
                Title = l.Item2,
                Items = items.Where(i => string.Equals(i.Difficulty, l.Item1, StringComparison.OrdinalIgnoreCase)).OrderBy(i => i.Order).ToList()
            })
            .Where(g => g.Items.Count > 0)
            .ToList();
        if (groups.Count == 0)
            groups.Add(new NavGroup { Title = "Exercises", Items = items.OrderBy(i => i.Order).ToList() });
        return new SectionNav { Section = ContentSectionType.Exercises, Groups = groups };
    }

    private static SectionNav BuildProjectsNav(List<ContentItem> items)
    {
        var overviews = items.Where(i => i.IsOverview).OrderBy(i => i.Order).ToList();
        var hasDayPages = items.Any(i => !i.IsOverview);

        // Ét dokument per projekt → flad liste (undgår gruppetitel + identisk link)
        if (!hasDayPages)
        {
            return new SectionNav
            {
                Section = ContentSectionType.Projects,
                Groups = [new NavGroup { Title = "", Items = overviews }]
            };
        }

        var groups = overviews.Select(o =>
        {
            var days = items.Where(i => i.ProjectSlug == o.Slug && !i.IsOverview).OrderBy(i => i.Order).ToList();
            return new NavGroup { Title = o.Title, Items = [o, ..days] };
        }).ToList();
        return new SectionNav { Section = ContentSectionType.Projects, Groups = groups };
    }
}
