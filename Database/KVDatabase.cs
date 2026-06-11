using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class KVDatabase
{
    private readonly string _path;
    private readonly List<KVEntry> _entries = new();

    public KVDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<KVEntry> All => _entries;

    public KVEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Key) == query ||
            Normalize(x.Category) == query);
    }

    public List<KVEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Key.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.CommonIssue.Contains(query, StringComparison.OrdinalIgnoreCase) ||
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
        var loaded = JsonSerializer.Deserialize<List<KVEntry>>(json);

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

    private static List<KVEntry> DefaultEntries()
    {
        return new()
        {
            new()
{
    Key = "Console ID",
    Category = "Identity",
    Meaning = "Unique identifier associated with a specific Xbox 360 console.",
    CommonIssue = "Mismatched identity data or corrupted console information.",
    Notes = "Reference-only console identity information."
},
new()
{
    Key = "Serial Number",
    Category = "Identity",
    Meaning = "Manufacturing serial number assigned to the console.",
    CommonIssue = "Incorrect or unreadable identity information.",
    Notes = "Useful for console identification and support."
},
new()
{
    Key = "Console Certificate",
    Category = "Security",
    Meaning = "Certificate used for console authentication and identity verification.",
    CommonIssue = "Invalid certificate data or authentication failures.",
    Notes = "Reference-only security component."
},
new()
{
    Key = "Manufacturing Date",
    Category = "Manufacturing",
    Meaning = "Date the console was manufactured.",
    CommonIssue = "Missing or corrupted metadata.",
    Notes = "Useful for motherboard and revision identification."
},
new()
{
    Key = "DVD Drive Key",
    Category = "Hardware",
    Meaning = "Value associated with optical drive pairing.",
    CommonIssue = "Pairing mismatch can cause disc-related issues.",
    Notes = "Reference-only hardware concept."
},
new()
{
    Key = "Region Code",
    Category = "Region",
    Meaning = "Defines the console's intended geographic market.",
    CommonIssue = "Region mismatch affecting content availability.",
    Notes = "Useful for compatibility research."
},
new()
{
    Key = "Console Type",
    Category = "Hardware",
    Meaning = "Identifies the console family or revision.",
    CommonIssue = "Incorrect identification information.",
    Notes = "Useful when researching motherboard revisions."
},
new()
{
    Key = "Privilege Flags",
    Category = "Security",
    Meaning = "Flags defining console capabilities and permissions.",
    CommonIssue = "Unexpected service restrictions.",
    Notes = "Reference-only privilege information."
},
new()
{
    Key = "Certificate Authority",
    Category = "Security",
    Meaning = "Authority associated with certificate trust validation.",
    CommonIssue = "Certificate verification issues.",
    Notes = "Reference-only security concept."
},
new()
{
    Key = "Console Certificate Expiry",
    Category = "Security",
    Meaning = "Certificate validity information.",
    CommonIssue = "Certificate-related authentication failures.",
    Notes = "Reference-only certificate metadata."
},
new()
{
    Key = "Console Region",
    Category = "Region",
    Meaning = "Region associated with console services and content.",
    CommonIssue = "Region-restricted content availability.",
    Notes = "Useful for marketplace troubleshooting."
},
new()
{
    Key = "ODD Pairing Data",
    Category = "Hardware",
    Meaning = "Data associated with optical disc drive pairing.",
    CommonIssue = "Drive communication or recognition issues.",
    Notes = "Reference-only hardware pairing concept."
},
new()
{
    Key = "Motherboard Revision",
    Category = "Hardware",
    Meaning = "Internal motherboard family identification.",
    CommonIssue = "Incorrect hardware identification.",
    Notes = "Useful when researching Xenon, Falcon, Jasper, Trinity, Corona, etc."
},
new()
{
    Key = "Security Sector Reference",
    Category = "Storage",
    Meaning = "Metadata associated with protected storage structures.",
    CommonIssue = "Storage integrity or recognition issues.",
    Notes = "Reference-only storage concept."
},
new()
{
    Key = "Service Authentication",
    Category = "Authentication",
    Meaning = "Information used during service authentication processes.",
    CommonIssue = "Xbox Live sign-in problems.",
    Notes = "Useful when researching authentication workflows."
},
new()
{
    Key = "Identity Verification",
    Category = "Authentication",
    Meaning = "Processes used to validate console identity.",
    CommonIssue = "Authentication or service access failures.",
    Notes = "Reference-only identity concept."
},
new()
{
    Key = "Media Authentication",
    Category = "Media",
    Meaning = "Validation associated with physical media access.",
    CommonIssue = "Disc recognition problems.",
    Notes = "Reference-only media validation concept."
},
new()
{
    Key = "Hardware Authentication",
    Category = "Hardware",
    Meaning = "Checks used to validate hardware identity and compatibility.",
    CommonIssue = "Unexpected hardware communication issues.",
    Notes = "Reference-only hardware concept."
},
new()
{
    Key = "Storage Authentication",
    Category = "Storage",
    Meaning = "Validation of connected storage devices.",
    CommonIssue = "Storage not recognized or corrupted.",
    Notes = "Useful for troubleshooting storage issues."
},
new()
{
    Key = "System Identity",
    Category = "Identity",
    Meaning = "Collection of values used to identify the console.",
    CommonIssue = "Identity-related authentication issues.",
    Notes = "Reference-only system identity concept."
}
        };
    }
}