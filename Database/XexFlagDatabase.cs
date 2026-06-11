using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class XexFlagDatabase
{
    private readonly List<XexFlagEntry> _entries = new()
    {
        new()
        {
            Name = "Retail Signed",
            Category = "Signature",
            Meaning = "Executable is signed for retail execution.",
            CommonUse = "Retail title executable.",
            Notes = "General XEX flag reference."
        },
        new()
        {
            Name = "Devkit Signed",
            Category = "Signature",
            Meaning = "Executable is intended for development hardware.",
            CommonUse = "Development and testing builds.",
            Notes = "Reference-only."
        },
        new()
        {
            Name = "Region Flags",
            Category = "Region",
            Meaning = "Executable may contain region/media compatibility flags.",
            CommonUse = "Compatibility and region checks.",
            Notes = "Useful for metadata reference."
        },
        new()
        {
            Name = "Media Flags",
            Category = "Media",
            Meaning = "Executable metadata related to disc, HDD, or package execution.",
            CommonUse = "Media compatibility.",
            Notes = "Useful for XEX metadata analysis."
        },
    };

    public IReadOnlyList<XexFlagEntry> All => _entries;

    public XexFlagEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == q || Normalize(x.Category) == q || Normalize(x.Meaning) == q || Normalize(x.CommonUse) == q);
    }

    public List<XexFlagEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Category.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) || x.CommonUse.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
