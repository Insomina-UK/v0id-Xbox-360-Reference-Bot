namespace XboxHresultBot.Entries;

public sealed class StatusCodeEntry
{
    public string Code { get; set; } = "";
    public string Category { get; set; } = "";
    public string Meaning { get; set; } = "";
    public string CommonCause { get; set; } = "";
    public string FixSuggestion { get; set; } = "";
}