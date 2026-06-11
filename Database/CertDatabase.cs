using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class CertDatabase
{
    private readonly List<CertEntry> _entries = new()
    {
        new()
        {
            Name = "Console Certificate",
            Area = "Identity",
            Meaning = "Certificate associated with console identity validation.",
            ValidationUse = "Used in trust and identity checks.",
            Notes = "Reference-only."
        },
        new()
        {
            Name = "Client Certificate",
            Area = "Authentication",
            Meaning = "Certificate used by client systems during authentication workflows.",
            ValidationUse = "Trust validation and service access.",
            Notes = "Reference-only."
        },
        new()
        {
            Name = "Certificate Chain",
            Area = "Security",
            Meaning = "Chain of trust for certificate validation.",
            ValidationUse = "Ensures certificates are signed by trusted authorities.",
            Notes = "Reference-only."
        },
    };

    public IReadOnlyList<CertEntry> All => _entries;

    public CertEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == q || Normalize(x.Area) == q || Normalize(x.Meaning) == q || Normalize(x.ValidationUse) == q);
    }

    public List<CertEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Area.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ValidationUse.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
