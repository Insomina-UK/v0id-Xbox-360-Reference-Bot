using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class RamDatabase
{
    private readonly List<RamEntry> _entries = new()
    {
        new()
        {
            Manufacturer = "Samsung",
            Type = "GDDR3",
            UsedOn = "Multiple Fat/Slim revisions",
            Capacity = "512MB total system memory",
            Notes = "Common Xbox 360 memory manufacturer."
        },
        new()
        {
            Manufacturer = "Hynix",
            Type = "GDDR3",
            UsedOn = "Multiple revisions",
            Capacity = "512MB total system memory",
            Notes = "Common memory package found on Xbox 360 boards."
        },
        new()
        {
            Manufacturer = "Elpida",
            Type = "GDDR3",
            UsedOn = "Some board revisions",
            Capacity = "512MB total system memory",
            Notes = "Memory package reference for board identification."
        },
        new()
        {
            Manufacturer = "Micron",
            Type = "GDDR3",
            UsedOn = "Some board revisions",
            Capacity = "512MB total system memory",
            Notes = "Memory package reference for repair and identification."
        },
    };

    public IReadOnlyList<RamEntry> All => _entries;

    public RamEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Manufacturer) == q || Normalize(x.Type) == q || Normalize(x.UsedOn) == q || Normalize(x.Capacity) == q);
    }

    public List<RamEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Manufacturer.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Type.Contains(query, StringComparison.OrdinalIgnoreCase) || x.UsedOn.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Capacity.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
