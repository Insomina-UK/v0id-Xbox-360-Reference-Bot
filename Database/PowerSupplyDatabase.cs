using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class PowerSupplyDatabase
{
    private readonly List<PowerSupplyEntry> _entries = new()
    {
        new()
        {
            Name = "203W Power Supply",
            Wattage = "203W",
            CompatibleBoards = "Xenon, Zephyr",
            ConnectorType = "Fat 203W connector",
            Voltage = "12V rail, 5V standby",
            Notes = "Launch-era high wattage supply. Can power lower wattage Fat consoles with compatible connector."
        },
        new()
        {
            Name = "175W Power Supply",
            Wattage = "175W",
            CompatibleBoards = "Falcon, Opus",
            ConnectorType = "Fat 175W connector",
            Voltage = "12V rail, 5V standby",
            Notes = "Used on 65nm CPU Fat boards."
        },
        new()
        {
            Name = "150W Power Supply",
            Wattage = "150W",
            CompatibleBoards = "Jasper, Tonasket",
            ConnectorType = "Fat 150W connector",
            Voltage = "12V rail, 5V standby",
            Notes = "Used on Jasper-family Fat boards."
        },
        new()
        {
            Name = "135W Power Supply",
            Wattage = "135W",
            CompatibleBoards = "Trinity",
            ConnectorType = "Slim power connector",
            Voltage = "12V rail, 5V standby",
            Notes = "Used on first Xbox 360 Slim revision."
        },
        new()
        {
            Name = "115W Power Supply",
            Wattage = "115W",
            CompatibleBoards = "Corona, Waitsburg, Stingray, Winchester",
            ConnectorType = "Slim/E power connector",
            Voltage = "12V rail, 5V standby",
            Notes = "Used on later Slim and Xbox 360 E revisions."
        },
    };

    public IReadOnlyList<PowerSupplyEntry> All => _entries;

    public PowerSupplyEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == q || Normalize(x.Wattage) == q || Normalize(x.CompatibleBoards) == q || Normalize(x.ConnectorType) == q);
    }

    public List<PowerSupplyEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Wattage.Contains(query, StringComparison.OrdinalIgnoreCase) || x.CompatibleBoards.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ConnectorType.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Voltage.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
