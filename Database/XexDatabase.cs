using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class XexDatabase
{
    private readonly List<XexEntry> _entries = new()
    {

        new()
        {
            Name = "Retail XEX",
            Game = "Generic",
            ShortName = "retail",
            TitleId = "",
            MediaId = "",
            Flags = "Retail executable, signed content",
            Libraries = "XAM, XAPI, XONLINE",
            Notes = "General Xbox 360 retail executable reference."
        },
        new()
        {
            Name = "Call of Duty 4 Default XEX",
            Game = "Call of Duty 4: Modern Warfare",
            ShortName = "cod4",
            TitleId = "415607E6",
            MediaId = "",
            Flags = "Retail title executable",
            Libraries = "XAM, XONLINE, XHTTP",
            Notes = "Reference entry for COD4 executable metadata."
        },
        new()
        {
            Name = "Modern Warfare 2 Default XEX",
            Game = "Call of Duty: Modern Warfare 2",
            ShortName = "mw2",
            TitleId = "41560817",
            MediaId = "",
            Flags = "Retail title executable",
            Libraries = "XAM, XONLINE, XHTTP",
            Notes = "Reference entry for MW2 executable metadata."
        },

    };

    public IReadOnlyList<XexEntry> All => _entries;

    public XexEntry? Find(string input)
    {
        string normalized = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == normalized || Normalize(x.Game) == normalized || Normalize(x.ShortName) == normalized || Normalize(x.TitleId) == normalized);
    }

    public List<XexEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Game.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ShortName.Contains(query, StringComparison.OrdinalIgnoreCase) || x.TitleId.Contains(query, StringComparison.OrdinalIgnoreCase) || x.MediaId.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Flags.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Libraries.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
