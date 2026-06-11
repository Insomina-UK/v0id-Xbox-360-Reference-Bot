using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class TitleUpdateDatabase
{
    private readonly List<TitleUpdateEntry> _entries = new()
    {

        new()
        {
            Game = "Call of Duty 4: Modern Warfare",
            ShortName = "cod4",
            TitleId = "415607E6",
            MediaId = "",
            Update = "TU4",
            Dashboard = "",
            Notes = "Common final update reference."
        },
        new()
        {
            Game = "Call of Duty: Modern Warfare 2",
            ShortName = "mw2",
            TitleId = "41560817",
            MediaId = "",
            Update = "TU9",
            Dashboard = "",
            Notes = "Popular Xbox 360 modding/reference title update."
        },
        new()
        {
            Game = "Call of Duty: Black Ops II",
            ShortName = "bo2",
            TitleId = "415608C3",
            MediaId = "",
            Update = "TU18",
            Dashboard = "",
            Notes = "Common final public update reference."
        },
        new()
        {
            Game = "Grand Theft Auto V",
            ShortName = "gta5",
            TitleId = "545408A7",
            MediaId = "",
            Update = "TU27",
            Dashboard = "",
            Notes = "Late Xbox 360 update reference."
        },

    };

    public IReadOnlyList<TitleUpdateEntry> All => _entries;

    public TitleUpdateEntry? Find(string input)
    {
        string normalized = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Game) == normalized || Normalize(x.ShortName) == normalized || Normalize(x.TitleId) == normalized || Normalize(x.MediaId) == normalized);
    }

    public List<TitleUpdateEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Game.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ShortName.Contains(query, StringComparison.OrdinalIgnoreCase) || x.TitleId.Contains(query, StringComparison.OrdinalIgnoreCase) || x.MediaId.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Update.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Dashboard.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
