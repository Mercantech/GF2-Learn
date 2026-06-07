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
        var groups = CurriculumCategoryCatalog.All
            .Select(c => new NavGroup
            {
                Title = c.Title,
                Items = items
                    .Where(i => string.Equals(
                        CurriculumCategoryCatalog.ResolveCategory(i),
                        c.Key,
                        StringComparison.OrdinalIgnoreCase))
                    .OrderBy(i => i.Order)
                    .ToList()
            })
            .Where(g => g.Items.Count > 0)
            .ToList();

        if (groups.Count == 0)
            groups.Add(new NavGroup { Title = "Curriculum", Items = items.OrderBy(i => i.Order).ToList() });

        return new SectionNav { Section = ContentSectionType.Curriculum, Groups = groups };
    }

    private static SectionNav BuildExercisesNav(List<ContentItem> items)
    {
        var categorized = items.Where(i => !string.IsNullOrWhiteSpace(i.Category)).ToList();
        if (categorized.Count > 0)
        {
            var groups = ExerciseCategoryCatalog.All
                .Select(c => new NavGroup
                {
                    Title = c.Title,
                    Items = categorized
                        .Where(i => string.Equals(i.Category, c.Key, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(i => i.Order)
                        .ToList()
                })
                .Where(g => g.Items.Count > 0)
                .ToList();

            var uncategorized = items
                .Where(i => string.IsNullOrWhiteSpace(i.Category))
                .OrderBy(i => i.Order)
                .ToList();
            if (uncategorized.Count > 0)
                groups.Add(new NavGroup { Title = "Øvrige", Items = uncategorized });

            return new SectionNav { Section = ContentSectionType.Exercises, Groups = groups };
        }

        var levels = new[] { ("begynder", "Beginner"), ("mellem", "Intermediate"), ("avanceret", "Advanced") };
        var levelGroups = levels
            .Select(l => new NavGroup
            {
                Title = l.Item2,
                Items = items.Where(i => string.Equals(i.Difficulty, l.Item1, StringComparison.OrdinalIgnoreCase)).OrderBy(i => i.Order).ToList()
            })
            .Where(g => g.Items.Count > 0)
            .ToList();
        if (levelGroups.Count == 0)
            levelGroups.Add(new NavGroup { Title = "Exercises", Items = items.OrderBy(i => i.Order).ToList() });
        return new SectionNav { Section = ContentSectionType.Exercises, Groups = levelGroups };
    }

    private static SectionNav BuildProjectsNav(List<ContentItem> items)
    {
        var overviews = items.Where(i => i.IsOverview).OrderBy(i => i.Order).ToList();
        var hasDayPages = items.Any(i => !i.IsOverview);

        if (!hasDayPages)
            return BuildProjectsNavByDifficulty(overviews);

        var groups = overviews.Select(o =>
        {
            var days = items.Where(i => i.ProjectSlug == o.Slug && !i.IsOverview).OrderBy(i => i.Order).ToList();
            return new NavGroup { Title = o.Title, Items = [o, ..days] };
        }).ToList();
        return new SectionNav { Section = ContentSectionType.Projects, Groups = groups };
    }

    private static SectionNav BuildProjectsNavByDifficulty(List<ContentItem> overviews)
    {
        var levels = new[] { ("begynder", "Begynder"), ("mellem", "Mellem"), ("avanceret", "Avanceret") };
        var groups = levels
            .Select(l => new NavGroup
            {
                Title = l.Item2,
                Items = overviews
                    .Where(i => string.Equals(i.Difficulty, l.Item1, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(i => i.Order)
                    .ToList()
            })
            .Where(g => g.Items.Count > 0)
            .ToList();

        if (groups.Count == 0)
            groups.Add(new NavGroup { Title = "", Items = overviews });

        return new SectionNav { Section = ContentSectionType.Projects, Groups = groups };
    }
}
