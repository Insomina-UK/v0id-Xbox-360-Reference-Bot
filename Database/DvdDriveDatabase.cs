using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class DvdDriveDatabase
{
    private readonly List<DvdDriveEntry> _entries = new()
    {

        new()
        {
            Name = "Hitachi-LG GDR-3120L",
            Manufacturer = "Hitachi-LG",
            Models = "46, 47, 58, 59, 78, 79",
            ConsoleType = "Xbox 360 Fat",
            KeyInfo = "Paired to console DVD key.",
            Notes = "Common in early Fat consoles."
        },
        new()
        {
            Name = "Samsung TS-H943",
            Manufacturer = "Samsung",
            Models = "MS25, MS28",
            ConsoleType = "Xbox 360 Fat",
            KeyInfo = "Paired to console DVD key.",
            Notes = "Seen in early Fat units."
        },
        new()
        {
            Name = "BenQ VAD6038",
            Manufacturer = "BenQ",
            Models = "VAD6038",
            ConsoleType = "Xbox 360 Fat",
            KeyInfo = "Paired to console DVD key.",
            Notes = "Common Fat DVD drive."
        },
        new()
        {
            Name = "LiteOn DG-16D2S",
            Manufacturer = "LiteOn",
            Models = "74850C, 83850C, 93450C",
            ConsoleType = "Xbox 360 Fat",
            KeyInfo = "Paired to console DVD key.",
            Notes = "Later Fat drive family."
        },
        new()
        {
            Name = "LiteOn DG-16D4S",
            Manufacturer = "LiteOn",
            Models = "9504, 0225, 0401, 1071",
            ConsoleType = "Xbox 360 Slim",
            KeyInfo = "Paired to console DVD key.",
            Notes = "Common Slim DVD drive family."
        },
        new()
        {
            Name = "Hitachi DL10N",
            Manufacturer = "Hitachi-LG",
            Models = "DL10N",
            ConsoleType = "Xbox 360 Slim / E",
            KeyInfo = "Paired to console DVD key.",
            Notes = "Used in some Slim/E consoles."
        },

    };

    public IReadOnlyList<DvdDriveEntry> All => _entries;

    public DvdDriveEntry? Find(string input)
    {
        string normalized = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == normalized || Normalize(x.Manufacturer) == normalized || Normalize(x.Models) == normalized || Normalize(x.ConsoleType) == normalized);
    }

    public List<DvdDriveEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Manufacturer.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Models.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ConsoleType.Contains(query, StringComparison.OrdinalIgnoreCase) || x.KeyInfo.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
