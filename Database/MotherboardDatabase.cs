using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class MotherboardDatabase
{
    private readonly string _path;
    private readonly List<MotherboardEntry> _entries = new();

    public MotherboardDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<MotherboardEntry> All => _entries;

    public MotherboardEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Name) == query ||
            Normalize(x.ConsoleType) == query);
    }

    public List<MotherboardEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.ConsoleType.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.ReleasePeriod.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.CpuGpu.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.PowerSupply.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Hdmi.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Nand.Contains(query, StringComparison.OrdinalIgnoreCase) ||
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
        var loaded = JsonSerializer.Deserialize<List<MotherboardEntry>>(json);

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
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "")
            .ToUpperInvariant();
    }

    private static List<MotherboardEntry> DefaultEntries()
    {
        return new()
        {
            new()
{
    Name = "Xenon",
    Codename = "Xenon",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2005",
    CpuGpu = "90nm CPU / 90nm GPU",
    PowerSupply = "203W",
    Hdmi = "No",
    Nand = "16MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "Yes",
    Reliability = "Poor",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/xenon.png",
    Notes = "Launch motherboard. No HDMI. Highest RROD failure rate."
},
new()
{
    Name = "Elpis",
    Codename = "Elpis",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2008",
    CpuGpu = "65nm CPU / 90nm GPU",
    PowerSupply = "175W",
    Hdmi = "No",
    Nand = "16MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "Yes",
    Reliability = "Good",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/elpis.png",
    Notes = "Rare Xenon replacement board used by Microsoft repair centers."
},
new()
{
    Name = "Zephyr",
    Codename = "Zephyr",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2007",
    CpuGpu = "90nm CPU / 90nm GPU",
    PowerSupply = "203W",
    Hdmi = "Yes",
    Nand = "16MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "Yes",
    Reliability = "Poor",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/zephyr.png",
    Notes = "First Xbox 360 motherboard revision with HDMI."
},
new()
{
    Name = "Falcon",
    Codename = "Falcon",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2007",
    CpuGpu = "65nm CPU / 90nm GPU",
    PowerSupply = "175W",
    Hdmi = "Yes",
    Nand = "16MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "Yes",
    Reliability = "Good",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/falcon.png",
    Notes = "First 65nm CPU revision."
},
           new()
{
    Name = "Opus",
    Codename = "Opus",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2008",
    CpuGpu = "65nm CPU / 90nm GPU",
    PowerSupply = "175W",
    Hdmi = "No",
    Nand = "16MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "Yes",
    Reliability = "Good",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/opus.png",
    Notes = "Falcon-style replacement board without HDMI, commonly used in Microsoft repair/refurb systems."
},
            new()
{
    Name = "Jasper",
    Codename = "Jasper",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2008",
    CpuGpu = "65nm CPU / 65nm GPU",
    PowerSupply = "150W",
    Hdmi = "Yes",
    Nand = "16MB / 256MB / 512MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "Yes",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/jasper.png",
    Notes = "Most reliable standard Fat motherboard."
},
new()
{
    Name = "Tonasket",
    Codename = "Tonasket",
    ConsoleType = "Xbox 360 Fat",
    ReleasePeriod = "2009",
    CpuGpu = "65nm CPU / 65nm GPU",
    PowerSupply = "150W",
    Hdmi = "Yes",
    Nand = "16MB / 256MB / 512MB",
    RghSupport = "RGH 1.2",
    JtagSupport = "No",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/fat.png",
    BoardImage = "Images/Motherboards/tonasket.png",
    Notes = "Jasper v2. Most reliable Fat motherboard produced."
},
new()
{
    Name = "Trinity",
    Codename = "Trinity",
    ConsoleType = "Xbox 360 Slim",
    ReleasePeriod = "2010",
    CpuGpu = "45nm XCGPU",
    PowerSupply = "135W",
    Hdmi = "Yes",
    Nand = "16MB",
    RghSupport = "RGH 1.2 / S-RGH",
    JtagSupport = "No",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/slim.png",
    BoardImage = "Images/Motherboards/trinity.png",
    Notes = "First Slim motherboard."
},
new()
{
    Name = "Corona",
    Codename = "Corona",
    ConsoleType = "Xbox 360 Slim",
    ReleasePeriod = "2011",
    CpuGpu = "45nm XCGPU",
    PowerSupply = "115W",
    Hdmi = "Yes",
    Nand = "16MB / 4GB",
    RghSupport = "RGH 3.0",
    JtagSupport = "No",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/slim.png",
    BoardImage = "Images/Motherboards/corona.png",
    Notes = "Most common Slim motherboard."
},
new()
{
    Name = "Waitsburg",
    Codename = "Waitsburg",
    ConsoleType = "Xbox 360 Slim",
    ReleasePeriod = "2013",
    CpuGpu = "45nm XCGPU",
    PowerSupply = "115W",
    Hdmi = "Yes",
    Nand = "4GB",
    RghSupport = "RGH 3.0",
    JtagSupport = "No",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/slim.png",
    BoardImage = "Images/Motherboards/waitsburg.png",
    Notes = "Late Slim motherboard revision before Stingray."
},
new()
{
    Name = "Stingray",
    Codename = "Stingray",
    ConsoleType = "Xbox 360 E",
    ReleasePeriod = "2013",
    CpuGpu = "45nm XCGPU",
    PowerSupply = "115W",
    Hdmi = "Yes",
    Nand = "4GB",
    RghSupport = "RGH 3.0",
    JtagSupport = "No",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/e.png",
    BoardImage = "Images/Motherboards/stingray.png",
    Notes = "Early Xbox 360 E motherboard revision."
},
new()
{
    Name = "Winchester",
    Codename = "Winchester",
    ConsoleType = "Xbox 360 E",
    ReleasePeriod = "2014",
    CpuGpu = "Integrated XCGPU",
    PowerSupply = "115W",
    Hdmi = "Yes",
    Nand = "4GB",
    RghSupport = "Not Supported",
    JtagSupport = "No",
    Reliability = "Excellent",
    ConsoleImage = "Images/Consoles/e.png",
    BoardImage = "Images/Motherboards/winchester.png",
    Notes = "Final Xbox 360 motherboard revision."
}
        };
    }
}