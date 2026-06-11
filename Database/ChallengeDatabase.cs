using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class ChallengeDatabase
{
    private readonly string _path;
    private readonly List<ChallengeEntry> _entries = new();

    public ChallengeDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<ChallengeEntry> All => _entries;

    public ChallengeEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Name) == query ||
            Normalize(x.Area) == query);
    }

    public List<ChallengeEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Area.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.SafeNotes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
        var loaded = JsonSerializer.Deserialize<List<ChallengeEntry>>(json);

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

    private static List<ChallengeEntry> DefaultEntries()
    {
        return new()
        {
           new()
{
    Name = "XSTS Token Validation",
    Area = "Authentication",
    Meaning = "Validation of Xbox service tokens used for authenticated service access.",
    SafeNotes = "Reference-only authentication concept."
},
new()
{
    Name = "Device Token Validation",
    Area = "Authentication",
    Meaning = "Checks associated with device-level authentication tokens.",
    SafeNotes = "Useful for documenting sign-in and device authentication flows."
},
new()
{
    Name = "User Token Validation",
    Area = "Authentication",
    Meaning = "Validation of user authentication state during service requests.",
    SafeNotes = "Reference-only account security term."
},
new()
{
    Name = "Session Token Validation",
    Area = "Authentication",
    Meaning = "Verification that a user or service session remains valid.",
    SafeNotes = "Useful when diagnosing expired session issues."
},
new()
{
    Name = "Refresh Token Validation",
    Area = "Authentication",
    Meaning = "Checks used when refreshing authentication state.",
    SafeNotes = "Reference-only service authentication term."
},
new()
{
    Name = "Account Region Validation",
    Area = "Account",
    Meaning = "Checks whether account region aligns with service or content requirements.",
    SafeNotes = "Useful for marketplace and region mismatch issues."
},
new()
{
    Name = "Account Age Validation",
    Area = "Account",
    Meaning = "Checks used to determine whether account age restrictions apply.",
    SafeNotes = "Reference-only parental and account control concept."
},
new()
{
    Name = "Family Settings Validation",
    Area = "Account",
    Meaning = "Validation of family safety restrictions and permissions.",
    SafeNotes = "Useful for diagnosing parental control blocks."
},
new()
{
    Name = "Privacy Settings Validation",
    Area = "Account",
    Meaning = "Checks that determine whether privacy settings allow a requested action.",
    SafeNotes = "Reference-only profile settings term."
},
new()
{
    Name = "Communication Permission Check",
    Area = "Account",
    Meaning = "Validation of whether messaging, voice, or multiplayer communication is allowed.",
    SafeNotes = "Useful for diagnosing social or party restrictions."
},
new()
{
    Name = "Multiplayer Permission Check",
    Area = "Account",
    Meaning = "Checks whether an account is allowed to access multiplayer services.",
    SafeNotes = "Reference-only Xbox Live permission concept."
},
new()
{
    Name = "Profile Sync Validation",
    Area = "Profile",
    Meaning = "Checks used when synchronizing profile data with Xbox Live.",
    SafeNotes = "Useful for diagnosing profile sync errors."
},
new()
{
    Name = "Profile Cache Validation",
    Area = "Profile",
    Meaning = "Validation of locally cached profile data.",
    SafeNotes = "Useful when clearing cache or redownloading profiles."
},
new()
{
    Name = "Profile Signature Validation",
    Area = "Profile",
    Meaning = "Integrity validation for profile-related data.",
    SafeNotes = "Reference-only profile integrity concept."
},
new()
{
    Name = "Profile Device Binding",
    Area = "Profile",
    Meaning = "Checks whether profile data is correctly associated with storage or device context.",
    SafeNotes = "Useful for diagnosing storage/profile access issues."
},
new()
{
    Name = "Achievement Sync Validation",
    Area = "Profile",
    Meaning = "Checks used when synchronizing achievement state.",
    SafeNotes = "Reference-only achievement sync concept."
},
new()
{
    Name = "Avatar Asset Validation",
    Area = "Profile",
    Meaning = "Validation of downloaded or cached avatar assets.",
    SafeNotes = "Useful for diagnosing avatar loading problems."
},
new()
{
    Name = "Motto/Bio Validation",
    Area = "Profile",
    Meaning = "Checks applied to profile text fields such as motto, bio, and location.",
    SafeNotes = "Reference-only profile moderation concept."
},
new()
{
    Name = "Reputation Data Validation",
    Area = "Profile",
    Meaning = "Validation of player reputation data synchronized with services.",
    SafeNotes = "Reference-only profile service concept."
},
new()
{
    Name = "Friends List Sync",
    Area = "Social",
    Meaning = "Checks used when loading or synchronizing friend list data.",
    SafeNotes = "Useful for diagnosing social service problems."
},
new()
{
    Name = "Party Session Validation",
    Area = "Social",
    Meaning = "Validation of Xbox Live party session state.",
    SafeNotes = "Reference-only social service term."
},
new()
{
    Name = "Voice Chat Permission Check",
    Area = "Social",
    Meaning = "Checks whether voice chat is allowed for the user/session.",
    SafeNotes = "Useful for diagnosing party or chat restrictions."
},
new()
{
    Name = "Message Service Validation",
    Area = "Social",
    Meaning = "Validation of messaging service availability and permissions.",
    SafeNotes = "Reference-only messaging service concept."
},
new()
{
    Name = "Invite Validation",
    Area = "Social",
    Meaning = "Checks whether game or party invites are valid.",
    SafeNotes = "Useful when diagnosing invite failure issues."
},
new()
{
    Name = "Presence Session Validation",
    Area = "Social",
    Meaning = "Validation of online presence and title activity data.",
    SafeNotes = "Reference-only presence service concept."
},
new()
{
    Name = "NAT Type Validation",
    Area = "Network",
    Meaning = "Checks used to determine network address translation status.",
    SafeNotes = "Useful for diagnosing multiplayer connectivity problems."
},
new()
{
    Name = "DNS Resolution Check",
    Area = "Network",
    Meaning = "Validation that service hostnames can resolve correctly.",
    SafeNotes = "Useful for diagnosing Xbox Live connection failures."
},
new()
{
    Name = "Gateway Reachability Check",
    Area = "Network",
    Meaning = "Checks whether the console can reach the configured gateway.",
    SafeNotes = "Useful for home network troubleshooting."
},
new()
{
    Name = "MTU Validation",
    Area = "Network",
    Meaning = "Validation that network packet size settings are compatible.",
    SafeNotes = "Useful for diagnosing connection test failures."
},
new()
{
    Name = "UPnP Validation",
    Area = "Network",
    Meaning = "Checks whether UPnP is available for service connectivity.",
    SafeNotes = "Reference-only router compatibility concept."
},
new()
{
    Name = "Port Availability Check",
    Area = "Network",
    Meaning = "Validation that required service ports are reachable.",
    SafeNotes = "Useful for diagnosing strict NAT or blocked service access."
},
new()
{
    Name = "Xbox Live Endpoint Check",
    Area = "Network",
    Meaning = "Checks whether Xbox Live service endpoints are reachable.",
    SafeNotes = "Reference-only connectivity diagnostic term."
},
new()
{
    Name = "Service Maintenance Check",
    Area = "Network",
    Meaning = "Validation of whether service maintenance or outage affects access.",
    SafeNotes = "Useful for distinguishing local issues from service outages."
},
new()
{
    Name = "Download Queue Validation",
    Area = "Download",
    Meaning = "Checks used when validating marketplace or content download queues.",
    SafeNotes = "Useful for diagnosing stuck downloads."
},
new()
{
    Name = "Partial Download Validation",
    Area = "Download",
    Meaning = "Validation of incomplete or interrupted downloaded content.",
    SafeNotes = "Useful for identifying damaged content packages."
},
new()
{
    Name = "Download Resume Check",
    Area = "Download",
    Meaning = "Checks whether a paused or interrupted download can resume.",
    SafeNotes = "Reference-only download management concept."
},
new()
{
    Name = "Marketplace Entitlement Check",
    Area = "Marketplace",
    Meaning = "Validation that the account has entitlement to marketplace content.",
    SafeNotes = "Use official account and purchase history tools for fixes."
},
new()
{
    Name = "Purchase History Validation",
    Area = "Marketplace",
    Meaning = "Checks used to confirm marketplace purchase ownership.",
    SafeNotes = "Useful for diagnosing unavailable purchased content."
},
new()
{
    Name = "Region Availability Check",
    Area = "Marketplace",
    Meaning = "Validation that content is available in the account or console region.",
    SafeNotes = "Reference-only marketplace availability term."
},
new()
{
    Name = "Delisted Content Check",
    Area = "Marketplace",
    Meaning = "Checks whether content is no longer available from marketplace services.",
    SafeNotes = "Useful for explaining unavailable legacy content."
},
new()
{
    Name = "Billing State Validation",
    Area = "Marketplace",
    Meaning = "Validation of billing state required for purchases or downloads.",
    SafeNotes = "Use official billing support for account payment issues."
},
new()
{
    Name = "License Cache Validation",
    Area = "Licensing",
    Meaning = "Checks locally cached license data for installed content.",
    SafeNotes = "Useful when troubleshooting content that will not launch."
},
new()
{
    Name = "Offline License Check",
    Area = "Licensing",
    Meaning = "Validation of whether content can be used while offline.",
    SafeNotes = "Reference-only license behavior concept."
},
new()
{
    Name = "Console License Check",
    Area = "Licensing",
    Meaning = "Checks whether content is licensed to the current console.",
    SafeNotes = "Use official license transfer options where applicable."
},
new()
{
    Name = "Account License Check",
    Area = "Licensing",
    Meaning = "Checks whether content is licensed to the signed-in account.",
    SafeNotes = "Useful for diagnosing content ownership issues."
},
new()
{
    Name = "DLC License Validation",
    Area = "Licensing",
    Meaning = "Validation of downloadable content ownership and compatibility.",
    SafeNotes = "Reference-only DLC troubleshooting concept."
},
new()
{
    Name = "Arcade Trial License Check",
    Area = "Licensing",
    Meaning = "Checks whether an Xbox Live Arcade title is trial or full version.",
    SafeNotes = "Useful for diagnosing XBLA trial/full unlock issues."
},
new()
{
    Name = "Content Region License Check",
    Area = "Licensing",
    Meaning = "Validation that licensed content is allowed in the current region.",
    SafeNotes = "Reference-only licensing availability concept."
},
new()
{
    Name = "Storage Device Authentication",
    Area = "Storage",
    Meaning = "Checks whether a storage device is recognized and usable.",
    SafeNotes = "Useful for diagnosing USB or HDD problems."
},
new()
{
    Name = "HDD Partition Validation",
    Area = "Storage",
    Meaning = "Validation of expected hard drive partition structure.",
    SafeNotes = "Reference-only storage layout concept."
},
new()
{
    Name = "USB Storage Validation",
    Area = "Storage",
    Meaning = "Checks whether USB storage is configured and compatible.",
    SafeNotes = "Useful for diagnosing external storage issues."
},
new()
{
    Name = "Content Index Validation",
    Area = "Storage",
    Meaning = "Validation of the content index used by the dashboard.",
    SafeNotes = "Useful for diagnosing missing or invisible content."
},
new()
{
    Name = "Cache Partition Validation",
    Area = "Storage",
    Meaning = "Checks locally cached dashboard and title data.",
    SafeNotes = "Clearing cache can resolve some related issues."
},
new()
{
    Name = "Save Container Validation",
    Area = "Storage",
    Meaning = "Integrity validation of save game containers.",
    SafeNotes = "Useful for diagnosing corrupted save errors."
},
new()
{
    Name = "Profile Container Validation",
    Area = "Storage",
    Meaning = "Integrity validation of profile storage containers.",
    SafeNotes = "Useful when profile data will not load."
},
new()
{
    Name = "Title Update Cache Check",
    Area = "Storage",
    Meaning = "Validation of cached title update data.",
    SafeNotes = "Useful for diagnosing title update conflicts."
},
new()
{
    Name = "Dashboard File Integrity",
    Area = "Dashboard",
    Meaning = "Checks whether dashboard files are intact and usable.",
    SafeNotes = "Useful for update or boot troubleshooting."
},
new()
{
    Name = "Dashboard Version Check",
    Area = "Dashboard",
    Meaning = "Validation of installed dashboard version against expected requirements.",
    SafeNotes = "Reference-only dashboard compatibility term."
},
new()
{
    Name = "System Settings Validation",
    Area = "Dashboard",
    Meaning = "Checks dashboard configuration and system settings data.",
    SafeNotes = "Useful for diagnosing settings corruption."
},
new()
{
    Name = "Locale Settings Validation",
    Area = "Dashboard",
    Meaning = "Validation of language, region, and locale settings.",
    SafeNotes = "Useful for region or marketplace issues."
},
new()
{
    Name = "Time/Date Validation",
    Area = "Dashboard",
    Meaning = "Checks system clock values used by services and certificates.",
    SafeNotes = "Incorrect time can cause authentication or service errors."
},
new()
{
    Name = "Update Package Integrity",
    Area = "Dashboard Update",
    Meaning = "Validation of dashboard update package contents.",
    SafeNotes = "Useful for diagnosing failed updates."
},
new()
{
    Name = "Update Version Compatibility",
    Area = "Dashboard Update",
    Meaning = "Checks whether an update package matches the console update path.",
    SafeNotes = "Reference-only update compatibility concept."
},
new()
{
    Name = "Update Storage Check",
    Area = "Dashboard Update",
    Meaning = "Validation that sufficient storage exists for system updates.",
    SafeNotes = "Useful when update install fails due to storage issues."
},
new()
{
    Name = "Update Signature Verification",
    Area = "Dashboard Update",
    Meaning = "Checks digital signatures on official update files.",
    SafeNotes = "Reference-only integrity term."
},
new()
{
    Name = "Title ID Validation",
    Area = "Title",
    Meaning = "Checks whether a title identifier matches expected metadata.",
    SafeNotes = "Useful for title reference and compatibility databases."
},
new()
{
    Name = "Title Metadata Validation",
    Area = "Title",
    Meaning = "Validation of game metadata used by dashboard and services.",
    SafeNotes = "Reference-only title management concept."
},
new()
{
    Name = "Title Update Compatibility",
    Area = "Title",
    Meaning = "Checks whether a title update applies to the installed game version.",
    SafeNotes = "Useful for diagnosing TU mismatch issues."
},
new()
{
    Name = "Title Save Compatibility",
    Area = "Title",
    Meaning = "Checks whether save data is compatible with the current title version.",
    SafeNotes = "Reference-only save compatibility term."
},
new()
{
    Name = "Title License Validation",
    Area = "Title",
    Meaning = "Validation that a title can be launched under current license conditions.",
    SafeNotes = "Useful for diagnosing launch failures."
},
new()
{
    Name = "XEX Header Validation",
    Area = "Executable",
    Meaning = "Checks metadata associated with Xbox executable files.",
    SafeNotes = "Reference-only executable format concept."
},
new()
{
    Name = "XEX Module Validation",
    Area = "Executable",
    Meaning = "Validation of loaded executable modules.",
    SafeNotes = "Useful for documenting module loading failures."
},
new()
{
    Name = "Import Table Validation",
    Area = "Executable",
    Meaning = "Checks executable import metadata required for loading.",
    SafeNotes = "Reference-only executable structure term."
},
new()
{
    Name = "Executable Region Check",
    Area = "Executable",
    Meaning = "Validation of regional compatibility metadata.",
    SafeNotes = "Useful for title compatibility reference."
},
new()
{
    Name = "Executable Media ID Check",
    Area = "Executable",
    Meaning = "Checks whether executable metadata matches expected media identifiers.",
    SafeNotes = "Reference-only title metadata concept."
},
new()
{
    Name = "Content Signature Validation",
    Area = "Security",
    Meaning = "Checks digital signatures for installed content packages.",
    SafeNotes = "Reference-only content integrity term."
},
new()
{
    Name = "System File Signature Check",
    Area = "Security",
    Meaning = "Validation of signatures on protected system files.",
    SafeNotes = "Useful for dashboard integrity documentation."
},
new()
{
    Name = "Certificate Revocation Check",
    Area = "Security",
    Meaning = "Checks whether certificate trust is still valid.",
    SafeNotes = "Reference-only certificate validation concept."
},
new()
{
    Name = "Console Serial Validation",
    Area = "Security",
    Meaning = "Checks console identity metadata consistency.",
    SafeNotes = "Reference-only console identity term."
},
new()
{
    Name = "Console ID Verification",
    Area = "Security",
    Meaning = "Validation of console identity values used by services.",
    SafeNotes = "Useful for documenting identity-related errors."
},
new()
{
    Name = "KeyVault Consistency Check",
    Area = "Security",
    Meaning = "Checks whether KeyVault-related identity data appears internally consistent.",
    SafeNotes = "Reference-only diagnostic concept."
},
new()
{
    Name = "FCRT Validation",
    Area = "Security",
    Meaning = "Validation associated with firmware certificate-related data.",
    SafeNotes = "Reference-only hardware/security term."
},
new()
{
    Name = "ODD Pairing Validation",
    Area = "Hardware",
    Meaning = "Checks optical disc drive pairing-related identity data.",
    SafeNotes = "Useful for hardware diagnostics and reference."
},
new()
{
    Name = "HDD Security Sector Check",
    Area = "Hardware",
    Meaning = "Validation of hard drive security sector data.",
    SafeNotes = "Reference-only storage hardware concept."
},
new()
{
    Name = "Thermal State Check",
    Area = "Hardware",
    Meaning = "Checks system thermal condition and safety state.",
    SafeNotes = "Useful for diagnosing overheating or shutdown behavior."
},
new()
{
    Name = "Fan State Validation",
    Area = "Hardware",
    Meaning = "Validation of fan status and cooling behavior.",
    SafeNotes = "Reference-only hardware monitoring concept."
},
new()
{
    Name = "Power State Validation",
    Area = "Hardware",
    Meaning = "Checks console power state transitions and hardware status.",
    SafeNotes = "Useful for diagnosing startup or shutdown issues."
},
new()
{
    Name = "Controller Authentication",
    Area = "Accessory",
    Meaning = "Validation of controller connection and accessory state.",
    SafeNotes = "Useful for diagnosing controller pairing issues."
},
new()
{
    Name = "Storage Accessory Validation",
    Area = "Accessory",
    Meaning = "Checks memory units and storage accessories for compatibility.",
    SafeNotes = "Reference-only accessory diagnostic concept."
},
new()
{
    Name = "Kinect Device Validation",
    Area = "Accessory",
    Meaning = "Validation of Kinect device connection and compatibility.",
    SafeNotes = "Useful for Kinect setup troubleshooting."
},
new()
{
    Name = "Headset Device Validation",
    Area = "Accessory",
    Meaning = "Checks headset and audio accessory connection state.",
    SafeNotes = "Useful for voice chat troubleshooting."
},
new()
{
    Name = "Media DRM Validation",
    Area = "Media",
    Meaning = "Checks playback permissions for protected media content.",
    SafeNotes = "Reference-only media licensing concept."
},
new()
{
    Name = "Video Codec Validation",
    Area = "Media",
    Meaning = "Checks whether media format and codec are supported.",
    SafeNotes = "Useful for diagnosing media playback errors."
},
new()
{
    Name = "Music Library Validation",
    Area = "Media",
    Meaning = "Validation of indexed music library content.",
    SafeNotes = "Reference-only media library concept."
},
new()
{
    Name = "Picture Library Validation",
    Area = "Media",
    Meaning = "Validation of indexed picture library content.",
    SafeNotes = "Useful for media browser troubleshooting."
},
new()
{
    Name = "Cloud Save Sync Validation",
    Area = "Cloud",
    Meaning = "Checks cloud save synchronization state.",
    SafeNotes = "Useful for diagnosing cloud save conflicts."
},
new()
{
    Name = "Cloud Storage Quota Check",
    Area = "Cloud",
    Meaning = "Validation of available cloud storage quota.",
    SafeNotes = "Reference-only cloud storage concept."
},
new()
{
    Name = "Cloud Conflict Resolution",
    Area = "Cloud",
    Meaning = "Checks whether local and cloud save versions conflict.",
    SafeNotes = "Useful for diagnosing save sync prompts."
},
new()
{
    Name = "Beacon Service Validation",
    Area = "Social",
    Meaning = "Validation of beacon/social activity service data.",
    SafeNotes = "Reference-only legacy social feature term."
},
new()
{
    Name = "Recommendation Service Check",
    Area = "Service",
    Meaning = "Checks availability of dashboard recommendation services.",
    SafeNotes = "Reference-only dashboard service term."
},
new()
{
    Name = "Advertisement Service Check",
    Area = "Service",
    Meaning = "Validation of dashboard advertisement service availability.",
    SafeNotes = "Useful for distinguishing service errors from console problems."
},
new()
{
    Name = "Telemetry Upload Check",
    Area = "Service",
    Meaning = "Checks whether diagnostic or usage telemetry can be submitted.",
    SafeNotes = "Reference-only service communication concept."
},
new()
{
    Name = "Service Policy Validation",
    Area = "Service",
    Meaning = "Checks whether current service policies allow the requested action.",
    SafeNotes = "Useful for service restriction documentation."
},
new()
{
    Name = "Ban State Check",
    Area = "Enforcement",
    Meaning = "Checks whether service access is restricted due to enforcement state.",
    SafeNotes = "Do not use this bot for bypassing enforcement."
},
new()
{
    Name = "Console Enforcement Check",
    Area = "Enforcement",
    Meaning = "Validation of console-level service eligibility.",
    SafeNotes = "Reference-only enforcement concept."
},
new()
{
    Name = "Account Enforcement Check",
    Area = "Enforcement",
    Meaning = "Validation of account-level restrictions.",
    SafeNotes = "Use official enforcement and account support channels."
},
new()
{
    Name = "Content Policy Check",
    Area = "Enforcement",
    Meaning = "Checks whether profile, message, or uploaded content violates policy.",
    SafeNotes = "Reference-only moderation concept."
},
new()
{
    Name = "Title Server Handshake",
    Area = "Title Server",
    Meaning = "Validation that a game title can communicate with its service endpoint.",
    SafeNotes = "Useful for diagnosing title-specific online service issues."
},
new()
{
    Name = "Title Server Session",
    Area = "Title Server",
    Meaning = "Checks active title service session state.",
    SafeNotes = "Reference-only title networking concept."
},
new()
{
    Name = "Leaderboard Sync Validation",
    Area = "Title Server",
    Meaning = "Validation of leaderboard upload or download state.",
    SafeNotes = "Useful for diagnosing leaderboard service issues."
},
new()
{
    Name = "Stats Upload Validation",
    Area = "Title Server",
    Meaning = "Checks whether title statistics can synchronize with services.",
    SafeNotes = "Reference-only title stats concept."
},
new()
{
    Name = "Anti-Cheat Policy Check",
    Area = "Title Server",
    Meaning = "Validation of title-specific fair play or service rules.",
    SafeNotes = "Reference-only. Do not use for bypassing restrictions."
}
        };
    }
}