using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class KernelDatabase
{
    private readonly List<KernelEntry> _entries = new()
    {
        new()
        {
            Version = "2.0.1888.0",
            ShortVersion = "1888",
            ReleaseDate = "22-11-2005",
            DashboardFamily = "Blades",
            Codename = "Launch Dashboard",
            Description = "Original Xbox 360 launch dashboard.",
            SecurityChanges = "Initial retail security baseline.",
            Hackability = "Historic reference only.",
            Notes = "Launch-era dashboard."
        },
        new()
        {
            Version = "2.0.4532.0",
            ShortVersion = "4532",
            ReleaseDate = "31-10-2006",
            DashboardFamily = "Blades",
            Codename = "Fall 2006 Update",
            Description = "Added HD DVD support and dashboard improvements.",
            SecurityChanges = "Early dashboard security updates.",
            Hackability = "Historic exploitable-era reference.",
            Notes = "Important early dashboard revision."
        },
        new()
        {
            Version = "2.0.4548.0",
            ShortVersion = "4548",
            ReleaseDate = "30-11-2006",
            DashboardFamily = "Blades",
            Codename = "Fall 2006 Patch",
            Description = "Minor dashboard patch release.",
            SecurityChanges = "Patched several early issues.",
            Hackability = "Historic exploitable-era reference.",
            Notes = "Early kernel reference."
        },
        new()
        {
            Version = "2.0.5759.0",
            ShortVersion = "5759",
            ReleaseDate = "09-05-2007",
            DashboardFamily = "Blades",
            Codename = "Spring 2007 Update",
            Description = "Added Windows Live Messenger and Marketplace updates.",
            SecurityChanges = "Service and dashboard hardening.",
            Hackability = "Historic reference.",
            Notes = "Blades-era update."
        },
        new()
        {
            Version = "2.0.6717.0",
            ShortVersion = "6717",
            ReleaseDate = "04-12-2007",
            DashboardFamily = "Blades",
            Codename = "Fall 2007 Update",
            Description = "Added Xbox Originals and dashboard improvements.",
            SecurityChanges = "Additional content and service checks.",
            Hackability = "Historic reference.",
            Notes = "Late Blades dashboard."
        },
        new()
        {
            Version = "2.0.7357.0",
            ShortVersion = "7357",
            ReleaseDate = "19-11-2008",
            DashboardFamily = "NXE",
            Codename = "New Xbox Experience",
            Description = "Major dashboard redesign introducing avatars and NXE UI.",
            SecurityChanges = "Major system update and dashboard architecture changes.",
            Hackability = "Pre-7371 era is commonly referenced for JTAG-era research.",
            Notes = "NXE launch dashboard."
        },
        new()
        {
            Version = "2.0.7371.0",
            ShortVersion = "7371",
            ReleaseDate = "02-04-2009",
            DashboardFamily = "NXE",
            Codename = "NXE Update",
            Description = "Important NXE-era dashboard revision.",
            SecurityChanges = "Late exploitable-era dashboard reference.",
            Hackability = "Common JTAG-era reference point.",
            Notes = "Frequently referenced in Xbox 360 history."
        },
        new()
        {
            Version = "2.0.8498.0",
            ShortVersion = "8498",
            ReleaseDate = "11-08-2009",
            DashboardFamily = "NXE",
            Codename = "Summer 2009 Update",
            Description = "Added Games on Demand and dashboard refinements.",
            SecurityChanges = "Post-JTAG-era protections became more common.",
            Hackability = "Not considered JTAG exploitable.",
            Notes = "Important post-7371 dashboard."
        },
        new()
        {
            Version = "2.0.9199.0",
            ShortVersion = "9199",
            ReleaseDate = "06-04-2010",
            DashboardFamily = "NXE",
            Codename = "USB Storage Update",
            Description = "Added USB storage support.",
            SecurityChanges = "System update and storage handling changes.",
            Hackability = "RGH-era reference only.",
            Notes = "Known for official USB storage support."
        },
        new()
        {
            Version = "2.0.12611.0",
            ShortVersion = "12611",
            ReleaseDate = "01-11-2010",
            DashboardFamily = "Kinect NXE",
            Codename = "Kinect Dashboard",
            Description = "Added Kinect support and redesigned dashboard elements.",
            SecurityChanges = "Major Kinect-era system changes.",
            Hackability = "RGH-era reference.",
            Notes = "Kinect launch dashboard."
        },
        new()
        {
            Version = "2.0.13146.0",
            ShortVersion = "13146",
            ReleaseDate = "19-05-2011",
            DashboardFamily = "Kinect NXE",
            Codename = "Disc Format Update",
            Description = "Updated disc support and system components.",
            SecurityChanges = "Disc authentication and system update changes.",
            Hackability = "RGH-era reference.",
            Notes = "Important disc format update."
        },
        new()
        {
            Version = "2.0.13599.0",
            ShortVersion = "13599",
            ReleaseDate = "19-07-2011",
            DashboardFamily = "Kinect NXE",
            Codename = "Summer 2011 Update",
            Description = "Dashboard stability and service improvements.",
            SecurityChanges = "Service hardening.",
            Hackability = "RGH-era reference.",
            Notes = "Common mid-generation kernel."
        },
        new()
        {
            Version = "2.0.14699.0",
            ShortVersion = "14699",
            ReleaseDate = "06-12-2011",
            DashboardFamily = "Metro",
            Codename = "Metro Dashboard",
            Description = "Introduced Metro-style dashboard UI.",
            SecurityChanges = "Dashboard and service layer changes.",
            Hackability = "RGH-era reference.",
            Notes = "Major UI redesign."
        },
        new()
        {
            Version = "2.0.14719.0",
            ShortVersion = "14719",
            ReleaseDate = "23-02-2012",
            DashboardFamily = "Metro",
            Codename = "Metro Patch",
            Description = "Metro dashboard update and service fixes.",
            SecurityChanges = "Minor security and service updates.",
            Hackability = "RGH-era reference.",
            Notes = "Metro-era update."
        },
        new()
        {
            Version = "2.0.15574.0",
            ShortVersion = "15574",
            ReleaseDate = "20-06-2012",
            DashboardFamily = "Metro",
            Codename = "Summer 2012 Update",
            Description = "System service and dashboard update.",
            SecurityChanges = "Key system security changes and bootloader updates.",
            Hackability = "RGH-era reference.",
            Notes = "Important security-era dashboard."
        },
        new()
        {
            Version = "2.0.16197.0",
            ShortVersion = "16197",
            ReleaseDate = "16-10-2012",
            DashboardFamily = "Metro",
            Codename = "Fall 2012 Update",
            Description = "Added Internet Explorer and dashboard refinements.",
            SecurityChanges = "System and service updates.",
            Hackability = "RGH-era reference.",
            Notes = "Metro-era dashboard."
        },
        new()
        {
            Version = "2.0.16202.0",
            ShortVersion = "16202",
            ReleaseDate = "27-11-2012",
            DashboardFamily = "Metro",
            Codename = "Metro Patch",
            Description = "Minor dashboard patch.",
            SecurityChanges = "Bug and service fixes.",
            Hackability = "RGH-era reference.",
            Notes = "Patch dashboard."
        },
        new()
        {
            Version = "2.0.16537.0",
            ShortVersion = "16537",
            ReleaseDate = "26-08-2013",
            DashboardFamily = "Metro",
            Codename = "2013 Update",
            Description = "Dashboard and system service update.",
            SecurityChanges = "Service and platform updates.",
            Hackability = "RGH-era reference.",
            Notes = "Late Xbox 360 dashboard."
        },
        new()
        {
            Version = "2.0.16747.0",
            ShortVersion = "16747",
            ReleaseDate = "12-12-2013",
            DashboardFamily = "Metro",
            Codename = "2013 Patch",
            Description = "Minor dashboard and service update.",
            SecurityChanges = "Security and service maintenance.",
            Hackability = "RGH-era reference.",
            Notes = "Late-generation update."
        },
        new()
        {
            Version = "2.0.17150.0",
            ShortVersion = "17150",
            ReleaseDate = "11-06-2014",
            DashboardFamily = "Metro",
            Codename = "2014 Update",
            Description = "Dashboard update with service improvements.",
            SecurityChanges = "Xbox Live and platform maintenance.",
            Hackability = "RGH-era reference.",
            Notes = "Late-generation dashboard."
        },
        new()
        {
            Version = "2.0.17349.0",
            ShortVersion = "17349",
            ReleaseDate = "30-04-2015",
            DashboardFamily = "Metro",
            Codename = "2015 Update",
            Description = "Dashboard update and service maintenance.",
            SecurityChanges = "Platform maintenance.",
            Hackability = "RGH-era reference.",
            Notes = "Late dashboard update."
        },
        new()
        {
            Version = "2.0.17489.0",
            ShortVersion = "17489",
            ReleaseDate = "17-09-2015",
            DashboardFamily = "Metro",
            Codename = "2015 System Update",
            Description = "System update and service maintenance.",
            SecurityChanges = "Security and service updates.",
            Hackability = "RGH-era reference.",
            Notes = "Common late dashboard."
        },
        new()
        {
            Version = "2.0.17502.0",
            ShortVersion = "17502",
            ReleaseDate = "17-11-2015",
            DashboardFamily = "Metro",
            Codename = "2015 Patch",
            Description = "Minor system update.",
            SecurityChanges = "Maintenance update.",
            Hackability = "RGH-era reference.",
            Notes = "Late dashboard patch."
        },
        new()
        {
            Version = "2.0.17511.0",
            ShortVersion = "17511",
            ReleaseDate = "17-11-2016",
            DashboardFamily = "Metro",
            Codename = "2016 Update",
            Description = "System maintenance update.",
            SecurityChanges = "Maintenance and service updates.",
            Hackability = "RGH-era reference.",
            Notes = "Late-generation dashboard."
        },
        new()
        {
            Version = "2.0.17526.0",
            ShortVersion = "17526",
            ReleaseDate = "22-05-2018",
            DashboardFamily = "Metro",
            Codename = "2018 Update",
            Description = "System maintenance update.",
            SecurityChanges = "Maintenance and service updates.",
            Hackability = "RGH-era reference.",
            Notes = "Very late Xbox 360 dashboard."
        },
        new()
        {
            Version = "2.0.17544.0",
            ShortVersion = "17544",
            ReleaseDate = "22-08-2019",
            DashboardFamily = "Metro",
            Codename = "2019 Update",
            Description = "System maintenance update.",
            SecurityChanges = "Maintenance and service updates.",
            Hackability = "RGH-era reference.",
            Notes = "Late official dashboard."
        },
        new()
        {
            Version = "2.0.17559.0",
            ShortVersion = "17559",
            ReleaseDate = "12-11-2019",
            DashboardFamily = "Metro",
            Codename = "Final Dashboard",
            Description = "Final public Xbox 360 dashboard update.",
            SecurityChanges = "Final public maintenance update.",
            Hackability = "Current final retail dashboard reference.",
            Notes = "Latest official public Xbox 360 dashboard."
        },
    };

    public IReadOnlyList<KernelEntry> All => _entries;

    public KernelEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Version) == q ||
            Normalize(x.ShortVersion) == q ||
            Normalize(x.Codename) == q ||
            Normalize(x.DashboardFamily) == q);
    }

    public List<KernelEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x =>
                x.Version.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.ShortVersion.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.ReleaseDate.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.DashboardFamily.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Codename.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToList();
    }

    public DashboardComparisonResult? Compare(string first, string second)
    {
        var a = Find(first);
        var b = Find(second);

        if (a == null || b == null)
            return null;

        return new DashboardComparisonResult
        {
            First = a,
            Second = b,
            Summary = $"{a.Version} compared with {b.Version}.",
            UpgradePath = BuildUpgradePath(a, b),
            KeyDifferences = BuildDifferences(a, b)
        };
    }

    private static string BuildUpgradePath(KernelEntry first, KernelEntry second)
    {
        int a = int.TryParse(first.ShortVersion, out int av) ? av : 0;
        int b = int.TryParse(second.ShortVersion, out int bv) ? bv : 0;

        if (a == b)
            return "Both entries reference the same kernel version.";

        return a < b
            ? $"{first.ShortVersion} → {second.ShortVersion}"
            : $"{second.ShortVersion} → {first.ShortVersion}";
    }

    private static string BuildDifferences(KernelEntry first, KernelEntry second)
    {
        return
            $"**Dashboard Family:** {first.DashboardFamily} vs {second.DashboardFamily}\n" +
            $"**Security:** {first.SecurityChanges} vs {second.SecurityChanges}\n" +
            $"**Hackability:** {first.Hackability} vs {second.Hackability}";
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("2.0.", "")
            .Replace(".0", "")
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "")
            .ToUpperInvariant();
    }
}
