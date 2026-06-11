using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class GpuDatabase
{
    private readonly List<GpuEntry> _entries = new()
    {
        new()
        {
            Name = "Xenos",
            Revision = "90nm",
            Process = "90nm GPU",
            UsedOn = "Xenon, Zephyr, Falcon/Opus GPU side",
            Notes = "Original Xbox 360 GPU family."
        },
        new()
        {
            Name = "Xenos",
            Revision = "65nm",
            Process = "65nm GPU",
            UsedOn = "Jasper, Tonasket",
            Notes = "Reduced process GPU used on more reliable Fat boards."
        },
        new()
        {
            Name = "XCGPU",
            Revision = "45nm",
            Process = "45nm combined CPU/GPU",
            UsedOn = "Trinity, Corona, Waitsburg, Stingray",
            Notes = "Combined CPU/GPU package used on Slim and E families."
        },
        new()
        {
            Name = "Integrated XCGPU",
            Revision = "Late E revision",
            Process = "Integrated package",
            UsedOn = "Winchester",
            Notes = "Final motherboard generation reference."
        },
    };

    public IReadOnlyList<GpuEntry> All => _entries;

    public GpuEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == q || Normalize(x.Revision) == q || Normalize(x.Process) == q || Normalize(x.UsedOn) == q);
    }

    public List<GpuEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Revision.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Process.Contains(query, StringComparison.OrdinalIgnoreCase) || x.UsedOn.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
