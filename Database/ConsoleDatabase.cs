using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class ConsoleDatabase
{
    private readonly List<ConsoleEntry> _entries = new()
    {
        new()
        {
            Name = "Xbox 360 Core",
            Family = "Fat",
            ReleaseDate = "2005",
            Motherboards = "Xenon, Zephyr, Falcon",
            Storage = "No HDD included",
            PowerSupply = "203W / 175W depending on revision",
            Hdmi = "No on early Xenon, Yes on later HDMI models",
            Finish = "White",
            Notes = "Entry-level launch package."
        },
        new()
        {
            Name = "Xbox 360 Premium",
            Family = "Fat",
            ReleaseDate = "2005",
            Motherboards = "Xenon, Zephyr, Falcon, Jasper",
            Storage = "20GB HDD originally",
            PowerSupply = "203W / 175W / 150W depending on board",
            Hdmi = "Depends on revision",
            Finish = "White with chrome tray",
            Notes = "Main early retail Xbox 360 package."
        },
        new()
        {
            Name = "Xbox 360 Arcade",
            Family = "Fat",
            ReleaseDate = "2007",
            Motherboards = "Falcon, Jasper, Tonasket",
            Storage = "Memory unit or internal memory depending on revision",
            PowerSupply = "175W / 150W",
            Hdmi = "Yes on most units",
            Finish = "White",
            Notes = "Budget model that replaced Core."
        },
        new()
        {
            Name = "Xbox 360 Elite",
            Family = "Fat",
            ReleaseDate = "2007",
            Motherboards = "Zephyr, Falcon, Jasper",
            Storage = "120GB / 250GB HDD depending on bundle",
            PowerSupply = "203W / 175W / 150W",
            Hdmi = "Yes",
            Finish = "Black",
            Notes = "Premium Fat model."
        },
        new()
        {
            Name = "Xbox 360 Slim 4GB",
            Family = "Slim",
            ReleaseDate = "2010",
            Motherboards = "Trinity, Corona",
            Storage = "4GB internal storage",
            PowerSupply = "135W / 115W",
            Hdmi = "Yes",
            Finish = "Black",
            Notes = "Slim model with internal flash storage."
        },
        new()
        {
            Name = "Xbox 360 Slim 250GB",
            Family = "Slim",
            ReleaseDate = "2010",
            Motherboards = "Trinity, Corona",
            Storage = "250GB removable internal HDD",
            PowerSupply = "135W / 115W",
            Hdmi = "Yes",
            Finish = "Gloss or matte black",
            Notes = "Main Xbox 360 S package."
        },
        new()
        {
            Name = "Xbox 360 E 4GB",
            Family = "E",
            ReleaseDate = "2013",
            Motherboards = "Stingray, Winchester",
            Storage = "4GB internal storage",
            PowerSupply = "115W",
            Hdmi = "Yes",
            Finish = "Xbox One-style black shell",
            Notes = "Late Xbox 360 E budget model."
        },
        new()
        {
            Name = "Xbox 360 E 250GB",
            Family = "E",
            ReleaseDate = "2013",
            Motherboards = "Stingray, Winchester",
            Storage = "250GB internal HDD",
            PowerSupply = "115W",
            Hdmi = "Yes",
            Finish = "Xbox One-style black shell",
            Notes = "Late Xbox 360 E HDD model."
        },
        new()
        {
            Name = "Xbox 360 Development Kit",
            Family = "Devkit",
            ReleaseDate = "2005+",
            Motherboards = "Development hardware revisions",
            Storage = "Dev HDD configurations",
            PowerSupply = "Varies",
            Hdmi = "Depends on revision",
            Finish = "Grey / special development shell",
            Notes = "Development hardware reference."
        },
        new()
        {
            Name = "Xbox 360 Test Kit",
            Family = "Test Kit",
            ReleaseDate = "2005+",
            Motherboards = "Test hardware revisions",
            Storage = "Test kit storage configurations",
            PowerSupply = "Varies",
            Hdmi = "Depends on revision",
            Finish = "Development/test shell",
            Notes = "Testing hardware reference."
        },
    };

    public IReadOnlyList<ConsoleEntry> All => _entries;

    public ConsoleEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Name) == q ||
            Normalize(x.Family) == q);
    }

    public List<ConsoleEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Family.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Motherboards.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Storage.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.PowerSupply.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToList();
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "")
            .ToUpperInvariant();
    }
}
