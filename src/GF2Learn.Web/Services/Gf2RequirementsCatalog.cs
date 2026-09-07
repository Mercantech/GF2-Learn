using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public static class Gf2RequirementsCatalog
{
    public static bool IsRequired(ContentItem item) => item.Section switch
    {
        ContentSectionType.Curriculum => item.Slug is
            "01-introduktion" or "02-variabler-og-datatyper" or
            "03-operatorer-og-udtryk" or "04-betingelser" or "05-loekker" or
            "13-ldap-active-directory" or "14-fejlfinding",
        ContentSectionType.Exercises => item.Slug is
            "01-variabler" or "02-input" or "03-control-flow" or "04-loops",
        _ => false
    };
}
