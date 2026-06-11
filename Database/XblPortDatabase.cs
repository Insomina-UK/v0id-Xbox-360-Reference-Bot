using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class XblPortDatabase
{
    private readonly List<XblPortEntry> _entries = new()
    {

        new()
        {
            Protocol = "UDP",
            Port = "88",
            Service = "Xbox Live Authentication",
            Direction = "Outbound/Inbound",
            Notes = "Used for Xbox Live authentication traffic."
        },
        new()
        {
            Protocol = "UDP/TCP",
            Port = "3074",
            Service = "Xbox Live Multiplayer",
            Direction = "Outbound/Inbound",
            Notes = "Primary Xbox Live multiplayer/NAT port."
        },
        new()
        {
            Protocol = "TCP",
            Port = "53",
            Service = "DNS",
            Direction = "Outbound",
            Notes = "Domain name resolution."
        },
        new()
        {
            Protocol = "UDP",
            Port = "53",
            Service = "DNS",
            Direction = "Outbound",
            Notes = "Domain name resolution."
        },
        new()
        {
            Protocol = "TCP",
            Port = "80",
            Service = "HTTP",
            Direction = "Outbound",
            Notes = "Marketplace, updates, and service connectivity."
        },
        new()
        {
            Protocol = "TCP",
            Port = "443",
            Service = "HTTPS",
            Direction = "Outbound",
            Notes = "Secure service communication."
        },

    };

    public IReadOnlyList<XblPortEntry> All => _entries;

    public XblPortEntry? Find(string input)
    {
        string normalized = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Protocol) == normalized || Normalize(x.Port) == normalized || Normalize(x.Service) == normalized || Normalize(x.Direction) == normalized);
    }

    public List<XblPortEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Protocol.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Port.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Service.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Direction.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
