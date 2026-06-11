using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class NandDatabase
{
    private readonly List<NandEntry> _entries = new()
    {

        new()
        {
            Name = "16MB Small Block",
            Boards = "Xenon, Zephyr, Falcon, Opus, Jasper, Trinity, Corona 16MB",
            Manufacturer = "Samsung / Hynix / others",
            Size = "16MB",
            Layout = "Small Block",
            BlockCount = "1024 blocks",
            Ecc = "16-byte ECC per page",
            BadBlockHandling = "Remap bad blocks according to image/tool output.",
            RecommendedTools = "JR Programmer, NAND-X, PicoFlasher",
            Notes = "Most common NAND size across Fat and Trinity boards."
        },
        new()
        {
            Name = "256MB Big Block",
            Boards = "Jasper Arcade",
            Manufacturer = "Samsung / Hynix / others",
            Size = "256MB",
            Layout = "Big Block",
            BlockCount = "Large block layout",
            Ecc = "Big block ECC layout",
            BadBlockHandling = "Bad block remapping must account for big block layout.",
            RecommendedTools = "JR Programmer, NAND-X",
            Notes = "Jasper internal memory variant."
        },
        new()
        {
            Name = "512MB Big Block",
            Boards = "Jasper Arcade",
            Manufacturer = "Samsung / Hynix / others",
            Size = "512MB",
            Layout = "Big Block",
            BlockCount = "Large block layout",
            Ecc = "Big block ECC layout",
            BadBlockHandling = "Bad block remapping must account for big block layout.",
            RecommendedTools = "JR Programmer, NAND-X",
            Notes = "Larger Jasper internal memory variant."
        },
        new()
        {
            Name = "4GB eMMC",
            Boards = "Corona 4GB, Waitsburg, Stingray, Winchester",
            Manufacturer = "eMMC package varies",
            Size = "4GB",
            Layout = "eMMC",
            BlockCount = "eMMC-managed",
            Ecc = "Managed internally by eMMC",
            BadBlockHandling = "Handled by eMMC controller.",
            RecommendedTools = "4GB read/write adapter, SD tool, compatible reader",
            Notes = "Common on later Slim/E consoles."
        },

    };

    public IReadOnlyList<NandEntry> All => _entries;

    public NandEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Name) == q || Normalize(x.Boards) == q || Normalize(x.Manufacturer) == q || Normalize(x.Size) == q);
    }

    public List<NandEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Boards.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Manufacturer.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Size.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Layout.Contains(query, StringComparison.OrdinalIgnoreCase) || x.BlockCount.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Ecc.Contains(query, StringComparison.OrdinalIgnoreCase) || x.BadBlockHandling.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
