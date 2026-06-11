using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class DvdFirmwareDatabase
{
    private readonly List<DvdFirmwareEntry> _entries = new()
    {
        new()
        {
            Drive = "Hitachi-LG GDR-3120L",
            Firmware = "32 / 36 / 40 / 46 / 47 / 58 / 59 / 78 / 79",
            ConsoleType = "Xbox 360 Fat",
            KeyStorage = "Drive firmware paired to console DVD key.",
            Notes = "Early Xbox 360 DVD drive family."
        },
        new()
        {
            Drive = "Samsung TS-H943",
            Firmware = "MS25 / MS28",
            ConsoleType = "Xbox 360 Fat",
            KeyStorage = "Drive firmware paired to console DVD key.",
            Notes = "Early Fat DVD drive family."
        },
        new()
        {
            Drive = "BenQ VAD6038",
            Firmware = "BenQ VAD6038",
            ConsoleType = "Xbox 360 Fat",
            KeyStorage = "Drive firmware paired to console DVD key.",
            Notes = "Common Fat DVD drive."
        },
        new()
        {
            Drive = "LiteOn DG-16D2S",
            Firmware = "74850C / 83850C / 93450C",
            ConsoleType = "Xbox 360 Fat",
            KeyStorage = "Drive firmware paired to console DVD key.",
            Notes = "Later Fat LiteOn drive family."
        },
        new()
        {
            Drive = "LiteOn DG-16D4S",
            Firmware = "9504 / 0225 / 0401 / 1071",
            ConsoleType = "Xbox 360 Slim",
            KeyStorage = "Drive firmware paired to console DVD key.",
            Notes = "Slim LiteOn drive family."
        },
    };

    public IReadOnlyList<DvdFirmwareEntry> All => _entries;

    public DvdFirmwareEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Drive) == q || Normalize(x.Firmware) == q || Normalize(x.ConsoleType) == q || Normalize(x.KeyStorage) == q);
    }

    public List<DvdFirmwareEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Drive.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Firmware.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ConsoleType.Contains(query, StringComparison.OrdinalIgnoreCase) || x.KeyStorage.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
