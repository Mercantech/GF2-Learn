namespace GF2Learn.Web.Client.Components.ProjectDemos.Jobsoegning;

public sealed class JobListing
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Company { get; set; } = "";
    public string Title { get; set; } = "";
    public string Status { get; set; } = "Ansøgt";
    public DateOnly Applied { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int DaysSinceApplied => Math.Max(0, DateOnly.FromDateTime(DateTime.Today).DayNumber - Applied.DayNumber);
}

public static class JobStatusStyles
{
    public static string CssClass(string status) => status switch
    {
        "Ansøgt" => "jt-badge jt-badge-applied",
        "Afventer" => "jt-badge jt-badge-waiting",
        "Afvist" => "jt-badge jt-badge-rejected",
        _ => "jt-badge"
    };
}
