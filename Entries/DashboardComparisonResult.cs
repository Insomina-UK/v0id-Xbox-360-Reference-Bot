namespace XboxHresultBot.Entries;

public sealed class DashboardComparisonResult
{
    public KernelEntry First { get; set; } = new();
    public KernelEntry Second { get; set; } = new();
    public string Summary { get; set; } = "";
    public string UpgradePath { get; set; } = "";
    public string KeyDifferences { get; set; } = "";
}
