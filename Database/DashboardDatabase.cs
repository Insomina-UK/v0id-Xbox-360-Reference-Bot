using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class DashboardDatabase
{
    private readonly string _path;
    private readonly List<DashboardEntry> _entries = new();

    public DashboardDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<DashboardEntry> All => _entries;

    public DashboardEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Version) == query ||
            Normalize(x.Family) == query);
    }

    public List<DashboardEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Version.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.ReleaseDate.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Family.Contains(query, StringComparison.OrdinalIgnoreCase) ||
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
        var loaded = JsonSerializer.Deserialize<List<DashboardEntry>>(json);

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

    private static List<DashboardEntry> DefaultEntries()
    {
        return new()
        {
            new()
            {
                Version = "2.0.1888.0",
                ReleaseDate = "22 Nov 2005",
                Family = "Blades",
                Description = "Original Xbox 360 launch dashboard.",
                Notes = "First public dashboard generation."
            },
            new()
            {
                Version = "2.0.2241.0",
                ReleaseDate = "22 Nov 2005",
                Family = "Blades",
                Description = "Launch day update.",
                Notes = "Early retail system update."
            },
            new()
            {
                Version = "2.0.4532.0",
                ReleaseDate = "2007",
                Family = "Blades",
                Description = "Early Blades-era dashboard revision.",
                Notes = "Legacy dashboard version."
            },
            new()
            {
                Version = "2.0.6717.0",
                ReleaseDate = "2008",
                Family = "Blades",
                Description = "Late Blades dashboard.",
                Notes = "One of the final Blades-era versions."
            },
            new()
            {
                Version = "2.0.7357.0",
                ReleaseDate = "Nov 2008",
                Family = "NXE",
                Description = "First New Xbox Experience dashboard.",
                Notes = "Introduced avatars and the redesigned dashboard."
            },
            new()
            {
                Version = "2.0.7371.0",
                ReleaseDate = "Nov 2008",
                Family = "NXE",
                Description = "NXE dashboard revision.",
                Notes = "Commonly referenced in legacy JTAG discussions."
            },
            new()
            {
                Version = "2.0.8498.0",
                ReleaseDate = "Aug 2009",
                Family = "NXE",
                Description = "NXE update with Games on Demand support.",
                Notes = "Popular late NXE build."
            },
            new()
            {
                Version = "2.0.9199.0",
                ReleaseDate = "2010",
                Family = "NXE",
                Description = "Final NXE dashboard generation.",
                Notes = "Pre-Kinect dashboard."
            },
            new()
            {
                Version = "2.0.12611.0",
                ReleaseDate = "Nov 2010",
                Family = "Kinect",
                Description = "First Kinect dashboard.",
                Notes = "Introduced Kinect-focused UI changes."
            },
            new()
            {
                Version = "2.0.14699.0",
                ReleaseDate = "Dec 2011",
                Family = "Metro",
                Description = "Metro dashboard introduced.",
                Notes = "Added Bing, YouTube, Cloud Saves, and Beacons."
            },
            new()
            {
                Version = "2.0.17559.0",
                ReleaseDate = "12 Nov 2019",
                Family = "Metro",
                Description = "Latest official Xbox 360 dashboard.",
                Notes = "Minor bug fixes and improvements."
            }
        };
    }
}