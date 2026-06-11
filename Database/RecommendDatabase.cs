using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class RecommendDatabase
{
    private readonly List<RecommendEntry> _entries = new()
    {

        new()
        {
            Name = "Corona RGH3",
            Category = "RGH",
            RecommendedFor = "Xbox 360 Slim Corona",
            Recommendation = "RGH3 with postfix adapter if required.",
            Notes = "Reference-only. Use for hardware identification and planning."
        },
        new()
        {
            Name = "Trinity RGH3",
            Category = "RGH",
            RecommendedFor = "Xbox 360 Slim Trinity",
            Recommendation = "RGH3 or S-RGH depending on preference.",
            Notes = "Trinity is commonly considered one of the easiest Slim boards to work with."
        },
        new()
        {
            Name = "Jasper RGH1.2",
            Category = "RGH",
            RecommendedFor = "Xbox 360 Fat Jasper",
            Recommendation = "RGH 1.2 using a compatible glitch chip.",
            Notes = "Jasper is one of the most reliable Fat revisions."
        },
        new()
        {
            Name = "Winchester",
            Category = "Hardware",
            RecommendedFor = "Xbox 360 E Winchester",
            Recommendation = "Not recommended for RGH; use as retail/reference hardware.",
            Notes = "Winchester is generally treated as unsupported for RGH."
        },

    };

    public IReadOnlyList<RecommendEntry> All => _entries;

    public RecommendEntry? Find(string input)
    {
        string normalized = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == normalized || Normalize(x.Category) == normalized || Normalize(x.RecommendedFor) == normalized || Normalize(x.Recommendation) == normalized);
    }

    public List<RecommendEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Category.Contains(query, StringComparison.OrdinalIgnoreCase) || x.RecommendedFor.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Recommendation.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
