using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class SouthbridgeDatabase
{
    private readonly List<SouthbridgeEntry> _entries = new()
    {
        new()
        {
            Name = "ANA",
            BoardFamily = "Early Fat",
            UsedOn = "Xenon",
            Role = "Video encoder and display output support.",
            CommonIssues = "Display faults, no video, scaler-related failures.",
            Notes = "Early non-HDMI video encoder reference."
        },
        new()
        {
            Name = "HANA",
            BoardFamily = "HDMI Fat",
            UsedOn = "Zephyr, Falcon, Jasper",
            Role = "HDMI/video encoder support.",
            CommonIssues = "HDMI/display faults, no video output.",
            Notes = "Commonly referenced on HDMI Fat boards."
        },
        new()
        {
            Name = "Southbridge",
            BoardFamily = "All Xbox 360 families",
            UsedOn = "Fat, Slim, E",
            Role = "I/O, storage, USB, system management, and peripheral control.",
            CommonIssues = "USB faults, boot issues, storage recognition problems.",
            Notes = "Important diagnostic area for hardware reference."
        },
    };

    public IReadOnlyList<SouthbridgeEntry> All => _entries;

    public SouthbridgeEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == q || Normalize(x.BoardFamily) == q || Normalize(x.UsedOn) == q || Normalize(x.Role) == q);
    }

    public List<SouthbridgeEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.BoardFamily.Contains(query, StringComparison.OrdinalIgnoreCase) || x.UsedOn.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Role.Contains(query, StringComparison.OrdinalIgnoreCase) || x.CommonIssues.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToList();
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("0x", "", StringComparison.OrdinalIgnoreCase)
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "")
            .ToUpperInvariant();
    }
}
