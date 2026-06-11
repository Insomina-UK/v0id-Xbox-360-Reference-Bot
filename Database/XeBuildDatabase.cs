using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class XeBuildDatabase
{
    private readonly string _path;
    private readonly List<XeBuildEntry> _entries = new();

    public XeBuildDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<XeBuildEntry> All => _entries;

    public XeBuildEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Version) == query ||
            Normalize(x.SupportedDashboard) == query);
    }

    public List<XeBuildEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Version.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.SupportedDashboard.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Type.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .ToList();
    }

    private void Load()
    {
        if (!File.Exists(_path))
        {
            _entries.AddRange(DefaultEntries());
            Save();
            return;
        }

        string json = File.ReadAllText(_path);
        var loaded = JsonSerializer.Deserialize<List<XeBuildEntry>>(json);

        if (loaded != null)
            _entries.AddRange(loaded);
    }

    private void Save()
    {
        File.WriteAllText(_path, JsonSerializer.Serialize(_entries, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("2.0.", "")
            .Replace(".0", "")
            .Replace(" ", "")
            .Replace("-", "")
            .ToUpperInvariant();
    }

    private static List<XeBuildEntry> DefaultEntries()
    {
        return new()
        {
            new()
            {
                Version = "xeBuild 1.21",
                SupportedDashboard = "2.0.17559.0",
                Type = "Reference",
                Description = "Commonly referenced xeBuild release for 17559 NAND image creation.",
                Notes = "Informational reference only."
            },
            new()
            {
                Version = "xeBuild 1.20",
                SupportedDashboard = "2.0.17544.0",
                Type = "Reference",
                Description = "xeBuild generation associated with dashboard 17544.",
                Notes = "Informational reference only."
            },
            new()
            {
                Version = "xeBuild 1.19",
                SupportedDashboard = "2.0.17526.0",
                Type = "Reference",
                Description = "xeBuild generation associated with dashboard 17526.",
                Notes = "Informational reference only."
            },
            new()
            {
                Version = "xeBuild 1.18",
                SupportedDashboard = "2.0.17489.0",
                Type = "Reference",
                Description = "xeBuild generation associated with dashboard 17489.",
                Notes = "Informational reference only."
            },
            new()
            {
                Version = "xeBuild 1.17",
                SupportedDashboard = "2.0.17349.0",
                Type = "Reference",
                Description = "xeBuild generation associated with dashboard 17349.",
                Notes = "Informational reference only."
            }
        };
    }
}