using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class StatusCodeDatabase
{
    private readonly string _path;
    private readonly List<StatusCodeEntry> _entries = new();

    public StatusCodeDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<StatusCodeEntry> All => _entries;

    public StatusCodeEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Code) == query);
    }

    public List<StatusCodeEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Code.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.CommonCause.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.FixSuggestion.Contains(query, StringComparison.OrdinalIgnoreCase))
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
        var loaded = JsonSerializer.Deserialize<List<StatusCodeEntry>>(json);

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
            .Replace("-", "")
            .Replace(" ", "")
            .ToUpperInvariant();
    }

    private static List<StatusCodeEntry> DefaultEntries()
    {
        return new()
        {
            new()
{
    Code = "80070002",
    Category = "Filesystem",
    Meaning = "The system cannot find the file specified.",
    CommonCause = "Missing file, deleted content, incorrect path, or damaged installation.",
    FixSuggestion = "Verify files exist, reinstall content, and check storage devices."
},
new()
{
    Code = "80070003",
    Category = "Filesystem",
    Meaning = "The system cannot find the specified path.",
    CommonCause = "Incorrect directory structure, disconnected storage device, or invalid path.",
    FixSuggestion = "Verify folder paths and reconnect storage devices."
},
new()
{
    Code = "80070005",
    Category = "Security",
    Meaning = "Access denied.",
    CommonCause = "Permission restrictions, account limitations, or protected content.",
    FixSuggestion = "Verify permissions, account ownership, and content licenses."
},
new()
{
    Code = "80070490",
    Category = "System",
    Meaning = "Element not found.",
    CommonCause = "Missing dashboard files, damaged content, or incomplete installation.",
    FixSuggestion = "Reinstall affected content or update the dashboard."
},
new()
{
    Code = "80072EF3",
    Category = "Network",
    Meaning = "Unable to contact Xbox Live services.",
    CommonCause = "DNS failure, internet outage, or Xbox Live service issue.",
    FixSuggestion = "Check internet connectivity and Xbox Live status."
},
new()
{
    Code = "80150010",
    Category = "Marketplace",
    Meaning = "Marketplace transaction failed.",
    CommonCause = "Billing issue, unavailable content, or marketplace outage.",
    FixSuggestion = "Verify payment methods and retry later."
},
new()
{
    Code = "80150017",
    Category = "Account",
    Meaning = "Account authentication failed.",
    CommonCause = "Incorrect credentials or Xbox Live account issue.",
    FixSuggestion = "Verify login details and account status."
},
new()
{
    Code = "80150048",
    Category = "Marketplace",
    Meaning = "Requested content is unavailable.",
    CommonCause = "Region restrictions, delisted content, or service issues.",
    FixSuggestion = "Verify content availability in your region."
},
new()
{
    Code = "80150080",
    Category = "Account",
    Meaning = "Account information is invalid.",
    CommonCause = "Corrupted profile data or account sync issue.",
    FixSuggestion = "Delete and redownload the profile."
},
new()
{
    Code = "80151006",
    Category = "Profile",
    Meaning = "Profile cannot be loaded.",
    CommonCause = "Corrupted profile, damaged storage, or authentication issue.",
    FixSuggestion = "Redownload the profile and test storage devices."
},
new()
{
    Code = "80151012",
    Category = "Profile",
    Meaning = "Profile authentication failed.",
    CommonCause = "Xbox Live profile verification issue.",
    FixSuggestion = "Clear cache and redownload the profile."
},
new()
{
    Code = "80151014",
    Category = "Profile",
    Meaning = "Profile download failed.",
    CommonCause = "Interrupted network connection or Xbox Live service issue.",
    FixSuggestion = "Retry profile download and verify network connectivity."
},
new()
{
    Code = "80151901",
    Category = "Network",
    Meaning = "Xbox Live service unavailable.",
    CommonCause = "Service outage or maintenance.",
    FixSuggestion = "Wait for Xbox Live services to recover."
},
new()
{
    Code = "8015190A",
    Category = "Network",
    Meaning = "Cannot connect to Xbox Live.",
    CommonCause = "Firewall, NAT, router, or DNS issue.",
    FixSuggestion = "Test network settings and restart networking equipment."
},
new()
{
    Code = "8015190E",
    Category = "Network",
    Meaning = "Network timeout occurred.",
    CommonCause = "Slow connection or packet loss.",
    FixSuggestion = "Improve network stability and retry."
},
new()
{
    Code = "80153003",
    Category = "Console Update",
    Meaning = "System update failed.",
    CommonCause = "Corrupted update package or interrupted installation.",
    FixSuggestion = "Download the update again and retry."
},
new()
{
    Code = "80153021",
    Category = "Console Update",
    Meaning = "Invalid update package.",
    CommonCause = "Incorrect dashboard files or corrupted update data.",
    FixSuggestion = "Verify update version and redownload files."
},
new()
{
    Code = "80154002",
    Category = "Account",
    Meaning = "Account creation error.",
    CommonCause = "Xbox Live service issue or invalid account information.",
    FixSuggestion = "Retry later and verify account details."
},
new()
{
    Code = "80154058",
    Category = "Account",
    Meaning = "Account region mismatch.",
    CommonCause = "Account region differs from console region settings.",
    FixSuggestion = "Verify console locale and account region."
},
new()
{
    Code = "80169D94",
    Category = "Download",
    Meaning = "Content download failed.",
    CommonCause = "Marketplace communication issue or interrupted transfer.",
    FixSuggestion = "Retry the download later."
},
new()
{
    Code = "807B0190",
    Category = "Network",
    Meaning = "Unable to obtain an IP address.",
    CommonCause = "DHCP failure or router issue.",
    FixSuggestion = "Restart networking equipment and renew network settings."
},
new()
{
    Code = "807B01F4",
    Category = "Network",
    Meaning = "DNS resolution failed.",
    CommonCause = "Router DNS issue or ISP outage.",
    FixSuggestion = "Use alternative DNS servers and retry."
},
new()
{
    Code = "807B01F7",
    Category = "Network",
    Meaning = "Gateway connection failed.",
    CommonCause = "Incorrect router configuration or network outage.",
    FixSuggestion = "Verify gateway settings and restart the router."
},
new()
{
    Code = "807B0500",
    Category = "Authentication",
    Meaning = "Authentication service unavailable.",
    CommonCause = "Xbox Live authentication outage.",
    FixSuggestion = "Retry later when services are restored."
},
new()
{
    Code = "807B10A6",
    Category = "Parental Controls",
    Meaning = "Content restricted by parental controls.",
    CommonCause = "Family settings preventing access.",
    FixSuggestion = "Adjust parental control settings if appropriate."
},
new()
{
    Code = "8015B000",
    Category = "Xbox Live",
    Meaning = "General Xbox Live service error.",
    CommonCause = "Service outage, account issue, or communication failure.",
    FixSuggestion = "Check Xbox Live status and retry later."
},
new()
{
    Code = "8015D021",
    Category = "Account",
    Meaning = "Account credentials are invalid.",
    CommonCause = "Incorrect email or password.",
    FixSuggestion = "Verify credentials and attempt sign-in again."
},
new()
{
    Code = "8015D086",
    Category = "Profile",
    Meaning = "Profile data is corrupted.",
    CommonCause = "Damaged profile storage data.",
    FixSuggestion = "Delete and redownload the Xbox Live profile."
},
new()
{
    Code = "8015D02E",
    Category = "Xbox Live",
    Meaning = "Xbox Live sign-in failed.",
    CommonCause = "Network issues, service outage, or account restrictions.",
    FixSuggestion = "Verify connectivity and Xbox Live account status."
}
        };
    }
}