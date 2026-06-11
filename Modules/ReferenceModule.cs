using Discord;
using Discord.Interactions;
using Microsoft.VisualBasic;
using Spectre.Console;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using System.Text.Json;
using XboxHresultBot.Database;
using XboxHresultBot.Entries;
using XboxHresultBot.Theme;

namespace XboxHresultBot.Modules;

public sealed class ReferenceModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly HresultDatabase _hresults;
    private readonly TitleIdDatabase _titleIds;
    private readonly StatusCodeDatabase _statusCodes;
    private readonly DashboardDatabase _dashboards;
    private readonly XeBuildDatabase _xeBuilds;
    private readonly KVDatabase _kv;
    private readonly ChallengeDatabase _challenges;
    private readonly MotherboardDatabase _motherboards;
    private readonly BotStatistics _stats;
    private readonly InteractionService _interactionService;
    private readonly RecommendDatabase _recommend;
    private readonly DvdDriveDatabase _dvdDrives;
    private readonly XblPortDatabase _xblPorts;
    private readonly NandDatabase _nand;
    private readonly TitleUpdateDatabase _titleUpdates;
    private readonly XexDatabase _xex;
    private readonly KernelDatabase _kernels;
    private readonly ConsoleDatabase _consoles;
    private readonly PowerSupplyDatabase _powerSupplies;
    private readonly SouthbridgeDatabase _southbridges;
    private readonly RamDatabase _ram;
    private readonly GpuDatabase _gpu;
    private readonly AchievementDatabase _achievements;
    private readonly CertDatabase _certs;
    private readonly XexFlagDatabase _xexFlags;
    private readonly DvdFirmwareDatabase _dvdFirmware;
    private readonly RepairDatabase _repairs;

    public ReferenceModule(
    HresultDatabase hresults,
    TitleIdDatabase titleIds,
    StatusCodeDatabase statusCodes,
    DashboardDatabase dashboards,
    XeBuildDatabase xeBuilds,
    KVDatabase kv,
    ChallengeDatabase challenges,
    MotherboardDatabase motherboards,
    BotStatistics stats,
    RecommendDatabase recommend,
    DvdDriveDatabase dvdDrives,
    XblPortDatabase xblPorts,
    NandDatabase nand,
    TitleUpdateDatabase titleUpdates,
    XexDatabase xex,
    KernelDatabase kernels,
    ConsoleDatabase consoles,

    PowerSupplyDatabase powerSupplies,
    SouthbridgeDatabase southbridges,
    RamDatabase ram,
    GpuDatabase gpu,
    AchievementDatabase achievements,
    CertDatabase certs,
    XexFlagDatabase xexFlags,
    DvdFirmwareDatabase dvdFirmware,

    RepairDatabase repairs,

    InteractionService interactionService)
    {
        _hresults = hresults;
        _titleIds = titleIds;
        _statusCodes = statusCodes;
        _dashboards = dashboards;
        _xeBuilds = xeBuilds;
        _kv = kv;
        _challenges = challenges;
        _motherboards = motherboards;
        _stats = stats;
        _recommend = recommend;
        _dvdDrives = dvdDrives;
        _xblPorts = xblPorts;
        _nand = nand;
        _titleUpdates = titleUpdates;
        _xex = xex;
        _kernels = kernels;
        _consoles = consoles;

        _powerSupplies = powerSupplies;
        _southbridges = southbridges;
        _ram = ram;
        _gpu = gpu;
        _achievements = achievements;
        _certs = certs;
        _xexFlags = xexFlags;
        _dvdFirmware = dvdFirmware;

        _repairs = repairs;

        _interactionService = interactionService;
    }

    [SlashCommand("lookup", "Look up an Xbox 360 HRESULT.")]
    public async Task LookupAsync(string code)
    {
        var entry = _hresults.Find(code);

        if (entry == null)
        {
            var results = _hresults.Search(code).Take(5).ToList();

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No HRESULT entry was found for `{code}`.\n\nTry `/search {code}` or `/list` to browse categories.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var suggestions = string.Join("\n", results.Select(x => $"`{x.Code}` — **{x.Name}**"));

            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed(
                        $"No exact HRESULT match found for `{code}`.\n\n**Closest matches:**\n{suggestions}")
                    .Build(),
                ephemeral: true);

            return;
        }

        var embed = EmbedTheme.Base(
                "🧩 HRESULT Lookup",
                $"### `{entry.Code}`\n**{entry.Name}**",
                EmbedTheme.Cyan)
            .AddField("📁 Category", SafeField(entry.Category), true)
            .AddField("🏷️ Name", $"`{SafeField(entry.Name)}`", true)
            .AddField("🧠 Meaning", SafeField(entry.Meaning), false)
            .AddField("⚠️ Common Cause", SafeField(entry.CommonCause), false)
            .AddField("🛠️ Fix Suggestion", SafeField(entry.FixSuggestion), false)
            .AddField(
                "🧭 Related Commands",
                $"`/search {entry.Category}`\n" +
                $"`/list`\n" +
                $"`/hresultcategory {entry.Category}`",
                false)
            .WithFooter($"HRESULT Database • {_hresults.All.Count:N0} entries")
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("search", "Search the Xbox 360 HRESULT database.")]
    public async Task SearchAsync(string query)
    {
        var results = _hresults.Search(query).Take(10).ToList();

        if (results.Count == 0)
        {
            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed(
                    $"No HRESULT results found for `{query}`.\n\nTry a code, category, or term like `logon`, `billing`, `dns`, `database`, `marketplace`, or `accounts`.")
                .Build(),
                ephemeral: true);

            return;
        }

        var embed = EmbedTheme.Base(
                "🔎 HRESULT Search Results",
                $"Search query: `{query}`\nShowing `{results.Count}` best result(s).",
                EmbedTheme.Cyan)
            .WithFooter($"HRESULT Database • {_hresults.All.Count:N0} entries");

        foreach (var result in results)
        {
            embed.AddField(
                $"`{result.Code}` — {result.Name}",
                $"**Category:** {SafeField(result.Category)}\n" +
                $"**Meaning:** {Shorten(SafeField(result.Meaning), 220)}\n" +
                $"**Fix:** {Shorten(SafeField(result.FixSuggestion), 180)}",
                false);
        }

        embed.AddField(
            "🧭 Tips",
            "`/lookup <code>` for exact details\n" +
            "`/list` for categories\n" +
            "`/hresultcategory <category>` for grouped browsing",
            false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("list", "List HRESULT database categories.")]
    public async Task ListAsync()
    {
        var groups = _hresults.All
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Category) ? "Uncategorised" : x.Category)
            .OrderByDescending(x => x.Count())
            .ToList();

        var total = _hresults.All.Count;
        var categoryCount = groups.Count;

        var embed = EmbedTheme.Base(
                "📂 HRESULT Database Categories",
                $"Total entries: `{total:N0}`\nCategories: `{categoryCount:N0}`",
                EmbedTheme.Cyan)
            .WithFooter("Use /hresultcategory <category> to browse a category");

        foreach (var group in groups.Take(24))
        {
            embed.AddField(
                $"📁 {group.Key}",
                $"`{group.Count():N0}` entries",
                true);
        }

        embed.AddField(
            "🔍 Example Searches",
            "`/search logon`\n" +
            "`/search billing`\n" +
            "`/search marketplace`\n" +
            "`/search database`\n" +
            "`/lookup 0x80151903`",
            false);

        await RespondAsync(embed: embed.Build());
    }

    private static string Shorten(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Not available";

        value = value.Trim();

        return value.Length <= maxLength
            ? value
            : value[..Math.Max(0, maxLength - 3)] + "...";
    }

    [SlashCommand("titleid", "Look up an Xbox 360 Title ID.")]
    public async Task TitleIdAsync(string query)
    {
        var entry = _titleIds.Find(query);

        if (entry == null)
        {
            var results = _titleIds.Search(query);

            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed(
                    $"No Title ID was found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var searchEmbed = EmbedTheme.SearchEmbed("🎮 Title ID Search Results", results.Count);

            foreach (var result in results.Take(10))
            {
                searchEmbed.AddField(
                    $"🎯 {result.Game}",
                    $"**Title ID:** `{result.TitleId}`\n" +
                    $"**Short Name:** `{result.ShortName}`\n" +
                    $"**Publisher:** {result.Publisher}",
                    false);
            }

            await RespondAsync(embed: searchEmbed.Build());
            return;
        }

        var embed = EmbedTheme.Base(
                "🎮 Xbox 360 Title ID",
                $"### {entry.Game}\n`{entry.TitleId}`",
                EmbedTheme.Success)
            .AddField("🏷️ Short Name", $"`{entry.ShortName}`", true)
            .AddField("🏢 Publisher", entry.Publisher, true)
            .AddField("📝 Notes", EmbedTheme.Empty(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("kernel", "Look up Xbox 360 kernel/dashboard version information.")]
    public async Task KernelAsync(string query)
    {
        _stats.TrackDatabaseLookup("Kernels");

        var entry = _kernels.Find(query);

        if (entry == null)
        {
            var results = _kernels.Search(query, 10);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed($"No kernel/dashboard entry found for `{query}`.").Build(),
                    ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("🧩 Kernel Search Results", results.Count);

            foreach (var result in results)
            {
                search.AddField(
                    $"{result.Version} — {result.Codename}",
                    $"**Family:** {result.DashboardFamily}\n**Released:** {result.ReleaseDate}\n**Notes:** {result.Notes}",
                    false);
            }

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base(
                "🧩 Xbox 360 Kernel Reference",
                $"### {entry.Version}\n`{entry.DashboardFamily}`",
                EmbedTheme.Orange)
            .AddField("🏷️ Codename", SafeField(entry.Codename), true)
            .AddField("📅 Release Date", SafeField(entry.ReleaseDate), true)
            .AddField("📦 Dashboard Family", SafeField(entry.DashboardFamily), true)
            .AddField("📖 Description", SafeField(entry.Description), false)
            .AddField("🛡️ Security Changes", SafeField(entry.SecurityChanges), false)
            .AddField("🔧 Hackability", SafeField(entry.Hackability), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .AddField("🧭 Related", $"`/dashboard {entry.ShortVersion}`\n`/dashboardcompare {entry.ShortVersion} 17559`", false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("kernels", "Browse Xbox 360 kernel/dashboard timeline.")]
    public async Task KernelsAsync()
    {
        _stats.TrackDatabaseLookup("Kernels");

        var embed = EmbedTheme.Base(
                "🧩 Xbox 360 Kernel Timeline",
                $"Current kernel entries: `{_kernels.All.Count:N0}`",
                EmbedTheme.Orange);

        foreach (var group in _kernels.All.GroupBy(x => x.DashboardFamily).OrderBy(x => x.Key))
        {
            string versions = string.Join(", ", group.Select(x => $"`{x.ShortVersion}`").Take(20));

            embed.AddField(
                $"📦 {group.Key}",
                versions,
                false);
        }

        embed.AddField(
            "🔍 Examples",
            "`/kernel 7371`\n`/kernel 17559`\n`/dashboardcompare 7371 17559`",
            false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("statuscode", "Look up an Xbox 360 status/error code.")]
    public async Task StatusCodeAsync(string code)
    {
        var entry = _statusCodes.Find(code);

        if (entry == null)
        {
            await RespondAsync(embed: EmbedTheme.ErrorEmbed(
                $"No status code was found for `{code}`.").Build(), ephemeral: true);
            return;
        }

        var embed = EmbedTheme.Base(
                "🚦 Xbox 360 Status Code",
                $"### `{entry.Code}`",
                EmbedTheme.Orange)
            .AddField("📁 Category", entry.Category, true)
            .AddField("🧠 Meaning", entry.Meaning, false)
            .AddField("⚠️ Common Cause", entry.CommonCause, false)
            .AddField("🛠️ Suggested Fix", entry.FixSuggestion, false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("kvinfo", "Show Xbox 360 KeyVault reference information.")]
    public async Task KvInfoAsync()
    {
        var embed = EmbedTheme.Base(
                "🔐 Xbox 360 KeyVault Reference",
                "Reference information about Xbox 360 KeyVault concepts, identity components, and diagnostic terminology.",
                EmbedTheme.Error)

            .AddField(
                "📖 What is a KeyVault?",
                "The KeyVault contains console-specific identity and security information used by the Xbox 360 operating system.",
                false)

            .AddField(
                "🆔 Identity Information",
                "• Console ID\n" +
                "• Serial Number\n" +
                "• Manufacturing Information\n" +
                "• Region Information\n" +
                "• Console Certificates",
                true)

            .AddField(
                "🔒 Security Components",
                "• Console Certificate\n" +
                "• Certificate Authority Data\n" +
                "• Security Flags\n" +
                "• Authentication Metadata\n" +
                "• Identity Verification",
                true)

            .AddField(
                "🔧 Hardware References",
                "• DVD Drive Pairing Data\n" +
                "• Motherboard Information\n" +
                "• Hardware Revision Data\n" +
                "• Storage Authentication",
                true)

            .AddField(
                "📂 Available KV Categories",
                "Identity\n" +
                "Security\n" +
                "Authentication\n" +
                "Hardware\n" +
                "Storage\n" +
                "Manufacturing\n" +
                "Region\n" +
                "Media\n" +
                "Certificates\n" +
                "System Information",
                false)

            .AddField(
                "⚠️ Common Issues",
                "• Console identity mismatch\n" +
                "• Authentication failures\n" +
                "• Region-related service restrictions\n" +
                "• Storage recognition issues\n" +
                "• Certificate validation errors",
                false)

            .AddField(
                "🔍 Useful Commands",
                "`/kv Console ID`\n" +
                "`/kv Serial Number`\n" +
                "`/kv Region Code`\n" +
                "`/kv DVD Drive Key`\n" +
                "`/kv Motherboard Revision`",
                false)

            .AddField(
                "📊 Database Coverage",
                $"Current KV Database Entries: `{_kv.All.Count}`",
                false)

            .AddField(
                "🛡️ Notice",
                "This bot provides reference and diagnostic information only. Information is intended for documentation, research, and community support purposes.",
                false)

            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("help", "Open the full Xbox 360 Reference Bot help center.")]
    public async Task HelpAsync()
    {
        var embed = EmbedTheme.Base(
                "📘 v0id Xbox 360 Reference Bot",
                "```ansi\n" +
                "\u001b[1;38;5;214mXbox 360 Reference Control Center\u001b[0m\n" +
                "\u001b[0;37mHRESULTs • Hardware • KV • XEX • Dashboards • Tools\u001b[0m\n" +
                "```",
                EmbedTheme.Orange)

            .AddField(
                "🔎 Error / HRESULT",
                "```txt\n" +
                "/lookup <code>              Exact HRESULT lookup\n" +
                "/search <query>             Search HRESULT database\n" +
                "/list                       HRESULT categories\n" +
                "/hresultcategory <category> Browse category\n" +
                "/statuscode <code>          Xbox status code lookup\n" +
                "/encyclopedia <query>          Search every database at once\n" +
                "```",
                false)

            .AddField(
                "🎮 Titles / Dashboards",
                "```txt\n" +
                "/titleid <game/titleid>      Title ID lookup\n" +
                "/tu <game/titleid>           Title update lookup\n" +
                "/kernel <version>            Kernel lookup\n" +
                "/kernels                     Kernel timeline\n" +
                "/dashboard <version>         Dashboard lookup\n" +
                "/dashboardcompare <a> <b>    Compare dashboards\n" +
                "/console <model>             Console model lookup\n" +
                "/consoles                    Console families\n" +
                "```",
                false)

            .AddField(
                "🔩 Hardware",
                "```txt\n" +
                "/motherboard <board>         Motherboard lookup\n" +
                "/motherboards                Motherboard database\n" +
                "/motherboardcompare <a> <b>  Compare boards\n" +
                "/powersupply <watt/board>    PSU compatibility\n" +
                "/dvddrive <drive>            DVD drive lookup\n" +
                "/dvdfirmware <drive>         DVD firmware lookup\n" +
                "/southbridge <ana/hana>      Southbridge info\n" +
                "/gpu <xenos/xcgpu>           GPU reference\n" +
                "/ram <manufacturer>          RAM reference\n" +
                "```",
                false)

            .AddField(
                "💾 NAND / RGH Reference",
                "```txt\n" +
                "/nand <query>                NAND lookup\n" +
                "/nandmega <query>            Detailed NAND layouts\n" +
                "/recommend <board/setup>     Recommended setup info\n" +
                "```",
                false)

            .AddField(
                "🔑 KV / Security / XEX",
                "```txt\n" +
                "/kv <query>                  KeyVault lookup\n" +
                "/kvinfo                      KeyVault overview\n" +
                "/kvfield <field>             KV field reference\n" +
                "/cert <query>                Certificate reference\n" +
                "/challenge <query>           Challenge lookup\n" +
                "/challenges                  Challenge areas\n" +
                "/xex <query>                 XEX metadata lookup\n" +
                "/xexflag <flag>              XEX flag reference\n" +
                "```",
                false)

            .AddField(
                "🌐 Xbox LIVE / Profile",
                "```txt\n" +
                "/xboxstatus                  Xbox LIVE status\n" +
                "/xblports                    Xbox LIVE ports\n" +
                "/avatarlookup <gamertag>     Avatar lookup\n" +
                "/avatardownload <gamertag>   Avatar image links\n" +
                "/profilepic <gamertag>       Profile/avatar picture\n" +
                "```",
                false)

            .AddField(
                "🏆 Achievements / Stats",
                "```txt\n" +
                "/achievement <game>          Achievement set lookup\n" +
                "/stats                       Bot/database statistics\n" +
                "/help                        Show this menu\n" +
                "```",
                false)

            .AddField(
                "⚡ Quick Examples",
                "`/lookup 0x80151903`\n" +
                "`/titleid mw2`\n" +
                "`/kernel 17559`\n" +
                "`/motherboard corona`\n" +
                "`/motherboardcompare trinity corona`\n" +
                "`/dvddrive liteon`\n" +
                "`/nand 4gb`\n" +
                "`/xex mw2`",
                false)

            .AddField(
                "📦 Databases Loaded",
                "`HRESULTs` `Title IDs` `Status Codes` `Dashboards` `Kernels` `Consoles`\n" +
                "`xeBuild` `KV` `Challenges` `Motherboards` `DVD` `NAND` `XEX` `Achievements`",
                false)

            .AddField(
                "🛡️ Notice",
                "This bot is for Xbox 360 reference, diagnostics, preservation, and educational documentation only.",
                false)

            .WithFooter("v0id Xbox 360 Reference Bot • Help Center")
            .WithCurrentTimestamp()
            .Build();

        await RespondAsync(embed: embed, ephemeral: true);
    }

    [SlashCommand("dashboard", "Look up an Xbox 360 dashboard/kernel version.")]
    public async Task DashboardAsync(string version)
    {
        var entry = _dashboards.Find(version);

        if (entry == null)
        {
            var results = _dashboards.Search(version);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No dashboard/kernel entry was found for `{version}`.\n\nTry examples like `17559`, `7371`, `NXE`, `Metro`, or `Blades`.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var searchEmbed = EmbedTheme.SearchEmbed(
                "🖥️ Dashboard Search Results",
                results.Count);

            foreach (var result in results.Take(10))
            {
                searchEmbed.AddField(
                    $"🧩 {result.Version}",
                    $"**Released:** {result.ReleaseDate}\n" +
                    $"**Family:** {result.Family}\n" +
                    $"**Summary:** {result.Description}",
                    false);
            }

            await RespondAsync(embed: searchEmbed.Build());
            return;
        }

        string quickTags = BuildDashboardTags(entry);

        var embed = EmbedTheme.Base(
                "🖥️ Xbox 360 Dashboard Reference",
                $"### {entry.Version}\n{quickTags}",
                EmbedTheme.Purple)
            .AddField("📅 Release Date", EmbedTheme.Empty(entry.ReleaseDate), true)
            .AddField("📦 Dashboard Family", EmbedTheme.Empty(entry.Family), true)
            .AddField("🔎 Lookup Alias", $"`{NormalizeDashboardAlias(entry.Version)}`", true)
            .AddField("📖 Description", EmbedTheme.Empty(entry.Description), false)
            .AddField("📝 Notes", EmbedTheme.Empty(entry.Notes), false)
            .AddField(
                "🧭 Related Commands",
                "`/kernel` — Full kernel timeline\n" +
                "`/xebuild " + NormalizeDashboardAlias(entry.Version) + "` — xeBuild reference\n" +
                "`/dashboard " + entry.Family + "` — Browse dashboard family",
                false)
            .Build();

        await RespondAsync(embed: embed);
    }

    private static string NormalizeDashboardAlias(string version)
    {
        return version
            .Replace("2.0.", "", StringComparison.OrdinalIgnoreCase)
            .Replace(".0", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
    }

    private static string BuildDashboardTags(DashboardEntry entry)
    {
        var tags = new List<string>();

        if (!string.IsNullOrWhiteSpace(entry.Family))
            tags.Add($"`{entry.Family}`");

        if (entry.Version.Contains("7371"))
            tags.Add("`JTAG-era`");

        if (entry.Version.Contains("12611"))
            tags.Add("`Kinect-era`");

        if (entry.Version.Contains("14699"))
            tags.Add("`Metro launch`");

        if (entry.Version.Contains("17559"))
            tags.Add("`Latest official`");

        if (tags.Count == 0)
            tags.Add("`Dashboard`");

        return string.Join(" ", tags);
    }

    [SlashCommand("dashboards", "Show all Xbox 360 dashboard families.")]
    public async Task DashboardsAsync()
    {
        var groups = _dashboards.All
            .GroupBy(x => x.Family)
            .OrderBy(x => x.Key)
            .ToList();

        var embed = EmbedTheme.Base(
                "🖥️ Xbox 360 Dashboard Families",
                $"Current dashboard database entries: `{_dashboards.All.Count}`",
                EmbedTheme.Purple);

        foreach (var group in groups)
        {
            var versions = group
                .OrderBy(x => x.Version)
                .Select(x => $"`{NormalizeDashboardAlias(x.Version)}`")
                .Take(15);

            embed.AddField(
                $"📦 {group.Key}",
                string.Join(", ", versions),
                false);
        }

        embed.AddField(
            "🔍 Examples",
            "`/dashboard 17559`\n`/dashboard 7371`\n`/dashboard NXE`\n`/dashboard Metro`",
            false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("dashboardcompare", "Compare two Xbox 360 dashboard/kernel versions.")]
    public async Task DashboardCompareAsync(string first, string second)
    {
        _stats.TrackDatabaseLookup("Dashboard Compare");

        var comparison = _kernels.Compare(first, second);

        if (comparison == null)
        {
            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed(
                    $"Could not compare `{first}` and `{second}` because one or both versions were not found.")
                .Build(),
                ephemeral: true);
            return;
        }

        var a = comparison.First;
        var b = comparison.Second;

        var embed = EmbedTheme.Base(
                "📊 Dashboard / Kernel Compare",
                $"### `{a.ShortVersion}` vs `{b.ShortVersion}`",
                EmbedTheme.Cyan)
            .AddField("📦 Version", $"**{a.ShortVersion}:** {a.Version}\n**{b.ShortVersion}:** {b.Version}", false)
            .AddField("📅 Release Date", $"**{a.ShortVersion}:** {a.ReleaseDate}\n**{b.ShortVersion}:** {b.ReleaseDate}", false)
            .AddField("🏷️ Codename", $"**{a.ShortVersion}:** {a.Codename}\n**{b.ShortVersion}:** {b.Codename}", false)
            .AddField("📦 Family", $"**{a.ShortVersion}:** {a.DashboardFamily}\n**{b.ShortVersion}:** {b.DashboardFamily}", false)
            .AddField("🛡️ Security", $"**{a.ShortVersion}:** {a.SecurityChanges}\n**{b.ShortVersion}:** {b.SecurityChanges}", false)
            .AddField("🔧 Hackability", $"**{a.ShortVersion}:** {a.Hackability}\n**{b.ShortVersion}:** {b.Hackability}", false)
            .AddField("🧭 Upgrade Path", comparison.UpgradePath, false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("xebuild", "Look up xeBuild reference info.")]
    public async Task XeBuildAsync(string query)
    {
        var entry = _xeBuilds.Find(query);

        if (entry == null)
        {
            var results = _xeBuilds.Search(query);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No xeBuild entry found for `{query}`.\n\nTry examples like `17559`, `17544`, `1.21`, or `xeBuild 1.21`.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var searchEmbed = EmbedTheme.SearchEmbed(
                "🧱 xeBuild Search Results",
                results.Count);

            foreach (var result in results.Take(10))
            {
                searchEmbed.AddField(
                    $"🧱 {result.Version}",
                    $"**Dashboard:** `{result.SupportedDashboard}`\n" +
                    $"**Type:** {result.Type}\n" +
                    $"**Summary:** {result.Description}",
                    false);
            }

            await RespondAsync(embed: searchEmbed.Build());
            return;
        }

        string dashboardAlias = NormalizeDashboardAlias(entry.SupportedDashboard);

        var embed = EmbedTheme.Base(
                "🧱 xeBuild Reference",
                $"### {entry.Version}\n`Dashboard {dashboardAlias}`",
                EmbedTheme.Primary)
            .AddField("🖥️ Supported Dashboard", entry.SupportedDashboard, true)
            .AddField("🏷️ Type", entry.Type, true)
            .AddField("🔎 Dashboard Alias", $"`{dashboardAlias}`", true)
            .AddField("📖 Description", EmbedTheme.Empty(entry.Description), false)
            .AddField("📝 Notes", EmbedTheme.Empty(entry.Notes), false)
            .AddField(
                "🧭 Related Commands",
                $"`/dashboard {dashboardAlias}` — Dashboard reference\n" +
                "`/dashboards` — View dashboard families\n" +
                "`/motherboard trinity` — Motherboard reference",
                false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("xebuilds", "Show all xeBuild database entries.")]
    public async Task XeBuildsAsync()
    {
        var entries = _xeBuilds.All
            .OrderByDescending(x => NormalizeDashboardAlias(x.SupportedDashboard))
            .ToList();

        var embed = EmbedTheme.Base(
                "🧱 xeBuild Database",
                $"Current xeBuild entries: `{entries.Count}`",
                EmbedTheme.Primary);

        foreach (var entry in entries.Take(20))
        {
            embed.AddField(
                $"🧱 {entry.Version}",
                $"**Dashboard:** `{entry.SupportedDashboard}`\n" +
                $"**Type:** {entry.Type}\n" +
                $"**Notes:** {EmbedTheme.Empty(entry.Notes)}",
                false);
        }

        embed.AddField(
            "🔍 Examples",
            "`/xebuild 17559`\n`/xebuild 1.21`\n`/xebuild xeBuild 1.21`",
            false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("kv", "Look up KeyVault reference info.")]
    public async Task KvAsync(string query)
    {
        var entry = _kv.Find(query);

        if (entry == null)
        {
            var results = _kv.Search(query);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No KeyVault entry was found for `{query}`.\n\nTry `Console ID`, `Certificate`, `Region`, `ODD`, or `Ban State`.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var searchEmbed = EmbedTheme.SearchEmbed("🔐 KV Search Results", results.Count);

            foreach (var result in results.Take(10))
            {
                searchEmbed.AddField(
                    $"🔐 {result.Key}",
                    $"**Category:** {result.Category}\n" +
                    $"**Meaning:** {result.Meaning}",
                    false);
            }

            await RespondAsync(embed: searchEmbed.Build());
            return;
        }

        var embed = EmbedTheme.Base(
                "🔐 KeyVault Reference",
                $"### {entry.Key}\n`{entry.Category}`",
                EmbedTheme.Error)
            .AddField("📁 Category", EmbedTheme.Empty(entry.Category), true)
            .AddField("🧠 Meaning", EmbedTheme.Empty(entry.Meaning), false)
            .AddField("⚠️ Common Issue", EmbedTheme.Empty(entry.CommonIssue), false)
            .AddField("📝 Notes", EmbedTheme.Empty(entry.Notes), false)
            .AddField("🧭 Related Commands", "`/kvinfo`\n`/kvcategories`\n`/kvcategory " + entry.Category + "`", false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("kvcategories", "List KeyVault categories.")]
    public async Task KvCategoriesAsync()
    {
        var groups = _kv.All
            .GroupBy(x => x.Category)
            .OrderBy(x => x.Key)
            .ToList();

        var embed = EmbedTheme.Base(
            "🔐 KeyVault Categories",
            $"Current KV database entries: `{_kv.All.Count}`",
            EmbedTheme.Error);

        foreach (var group in groups)
        {
            embed.AddField(
                $"📁 {group.Key}",
                $"{group.Count()} entries",
                true);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("kvcategory", "Browse KeyVault entries by category.")]
    public async Task KvCategoryAsync(string category)
    {
        var results = _kv.All
            .Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Key)
            .Take(20)
            .ToList();

        if (results.Count == 0)
        {
            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed($"No KV category found for `{category}`.").Build(),
                ephemeral: true);

            return;
        }

        var embed = EmbedTheme.Base(
            $"🔐 KV Category: {category}",
            $"Showing `{results.Count}` entries.",
            EmbedTheme.Error);

        foreach (var item in results)
        {
            embed.AddField(
                item.Key,
                item.Meaning,
                false);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("challenge", "Look up Xbox 360 challenge reference terms.")]
    public async Task ChallengeAsync(string query)
    {
        var entry = _challenges.Find(query);

        if (entry == null)
        {
            var results = _challenges.Search(query);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No challenge entry found for `{query}`.\n\nTry `XOSC`, `AP25`, `Profile Auth`, `Content License`, or `Title Update`.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var searchEmbed = EmbedTheme.SearchEmbed("🧪 Challenge Search Results", results.Count);

            foreach (var result in results.Take(10))
            {
                searchEmbed.AddField(
                    $"🧪 {result.Name}",
                    $"**Area:** {result.Area}\n" +
                    $"**Meaning:** {result.Meaning}",
                    false);
            }

            await RespondAsync(embed: searchEmbed.Build());
            return;
        }

        var embed = EmbedTheme.Base(
                "🧪 Xbox 360 Challenge Reference",
                $"### {entry.Name}\n`{entry.Area}`",
                EmbedTheme.Warning)
            .AddField("📁 Area", EmbedTheme.Empty(entry.Area), true)
            .AddField("🧠 Meaning", EmbedTheme.Empty(entry.Meaning), false)
            .AddField("🛡️ Safe Notes", EmbedTheme.Empty(entry.SafeNotes), false)
            .AddField("🧭 Related Commands", "`/challenges`\n`/challengearea " + entry.Area + "`", false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("challenges", "List Xbox 360 challenge reference areas.")]
    public async Task ChallengesAsync()
    {
        var groups = _challenges.All
            .GroupBy(x => x.Area)
            .OrderBy(x => x.Key)
            .ToList();

        var embed = EmbedTheme.Base(
            "🧪 Challenge Reference Areas",
            $"Current challenge entries: `{_challenges.All.Count}`",
            EmbedTheme.Warning);

        foreach (var group in groups.Take(25))
        {
            embed.AddField(
                $"📁 {group.Key}",
                $"{group.Count()} entries",
                true);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("challengearea", "Browse challenge entries by area.")]
    public async Task ChallengeAreaAsync(string area)
    {
        var results = _challenges.All
            .Where(x => x.Area.Equals(area, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name)
            .Take(20)
            .ToList();

        if (results.Count == 0)
        {
            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed($"No challenge area found for `{area}`.").Build(),
                ephemeral: true);

            return;
        }

        var embed = EmbedTheme.Base(
            $"🧪 Challenge Area: {area}",
            $"Showing `{results.Count}` entries.",
            EmbedTheme.Warning);

        foreach (var item in results)
        {
            embed.AddField(
                item.Name,
                item.Meaning,
                false);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("motherboard", "Look up an Xbox 360 motherboard.")]
    public async Task MotherboardAsync(string query)
    {
        var entry = _motherboards.Find(query);

        if (entry == null)
        {
            var results = _motherboards.Search(query);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No motherboard found for `{query}`.\n\nTry `Xenon`, `Zephyr`, `Falcon`, `Opus`, `Jasper`, `Trinity`, `Corona`, or `Winchester`.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var searchEmbed = EmbedTheme.SearchEmbed(
                "🔩 Motherboard Search Results",
                results.Count);

            foreach (var result in results.Take(10))
            {
                searchEmbed.AddField(
                    $"🔩 {result.Name}",
                    $"**Console:** {EmbedTheme.Empty(result.ConsoleType)}\n" +
                    $"**Release:** {EmbedTheme.Empty(result.ReleasePeriod)}\n" +
                    $"**Power:** {EmbedTheme.Empty(result.PowerSupply)}\n" +
                    $"**NAND:** {EmbedTheme.Empty(result.Nand)}",
                    false);
            }

            await RespondAsync(embed: searchEmbed.Build());
            return;
        }

        string consoleImagePath = ResolveLocalPath(entry.ConsoleImage);
        string boardImagePath = ResolveLocalPath(entry.BoardImage);

        bool hasConsoleImage =
            !string.IsNullOrWhiteSpace(consoleImagePath) &&
            File.Exists(consoleImagePath);

        bool hasBoardImage =
            !string.IsNullOrWhiteSpace(boardImagePath) &&
            File.Exists(boardImagePath);

        var files = new List<FileAttachment>();

        if (hasConsoleImage)
            files.Add(new FileAttachment(consoleImagePath, "console.png"));

        if (hasBoardImage)
            files.Add(new FileAttachment(boardImagePath, "board.png"));

        var embedBuilder = EmbedTheme.Base(
        "🔩 Xbox 360 Motherboard Reference",
        $"### {SafeField(entry.Name)}",
        EmbedTheme.Gold)
    .AddField("🏷️ Codename", SafeField(entry.Codename), true)
    .AddField("🎮 Console Type", SafeField(entry.ConsoleType), true)
    .AddField("📅 Release Period", SafeField(entry.ReleasePeriod), true)
    .AddField("🧠 CPU / GPU", SafeField(entry.CpuGpu), false)
    .AddField("🔌 Power Supply", SafeField(entry.PowerSupply), true)
    .AddField("📺 HDMI", SafeField(entry.Hdmi), true)
    .AddField("💾 NAND", SafeField(entry.Nand), true)
    .AddField("⚡ RGH Support", SafeField(entry.RghSupport), true)
    .AddField("🔓 JTAG Support", SafeField(entry.JtagSupport), true)
    .AddField("📈 Reliability", SafeField(GetReliabilityRating(entry.Reliability)), true)
    .AddField("🖼️ Images", BuildImageStatus(hasConsoleImage, hasBoardImage), false)
    .AddField("📝 Notes", SafeField(entry.Notes), false)
    .AddField("🧭 Related Revisions", SafeField(GetRelatedMotherboards(entry.Name)), false)
    .AddField("📚 Quick Facts", SafeField(GetMotherboardFacts(entry.Name)), false);

        if (hasConsoleImage)
            embedBuilder.WithThumbnailUrl("attachment://console.png");

        if (hasBoardImage)
            embedBuilder.WithImageUrl("attachment://board.png");

        var embed = embedBuilder.Build();

        try
        {
            if (files.Count > 0)
            {
                await RespondWithFilesAsync(
                    attachments: files,
                    embed: embed);
            }
            else
            {
                await RespondAsync(embed: embed);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Motherboard command error: {ex}");

            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed(
                    "Motherboard lookup loaded, but one or more image files failed to attach.\n\n" +
                    "Check that the image files are set to `Copy if newer` and exist inside `bin/Debug/net8.0/Images/...`.")
                .Build(),
                ephemeral: true);
        }
    }

    private static string SafeField(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "Not available"
            : value;
    }

    private static string GetReliabilityRating(string? rating)
    {
        if (string.IsNullOrWhiteSpace(rating))
            return "Not available";

        return rating switch
        {
            "Poor" => "🔴 Poor",
            "Good" => "🟡 Good",
            "Excellent" => "🟢 Excellent",
            _ => rating
        };
    }

    private static string BuildImageStatus(bool hasConsoleImage, bool hasBoardImage)
    {
        return
            $"{(hasConsoleImage ? "✅ Console image loaded" : "⚠️ Console image missing")}\n" +
            $"{(hasBoardImage ? "✅ Motherboard image loaded" : "⚠️ Motherboard image missing")}";
    }

    private static string GetRelatedMotherboards(string board)
    {
        return board switch
        {
            "Xenon" => "`Elpis` `Zephyr`",
            "Elpis" => "`Xenon` `Zephyr`",
            "Zephyr" => "`Xenon` `Falcon`",
            "Falcon" => "`Opus` `Jasper`",
            "Opus" => "`Falcon` `Jasper`",
            "Jasper" => "`Tonasket` `Falcon`",
            "Tonasket" => "`Jasper`",
            "Trinity" => "`Corona`",
            "Corona" => "`Trinity` `Waitsburg`",
            "Waitsburg" => "`Corona` `Stingray`",
            "Stingray" => "`Waitsburg` `Winchester`",
            "Winchester" => "`Stingray`",
            _ => "None"
        };
    }

    private static string GetMotherboardFacts(string board)
    {
        return board switch
        {
            "Xenon" =>
                "• Launch motherboard\n" +
                "• No HDMI\n" +
                "• Highest RROD rate",

            "Elpis" =>
                "• Microsoft repair replacement\n" +
                "• Rare revision\n" +
                "• Xenon replacement",

            "Zephyr" =>
                "• First HDMI motherboard\n" +
                "• Still 90nm CPU/GPU",

            "Falcon" =>
                "• First 65nm CPU\n" +
                "• Lower power usage",

            "Opus" =>
                "• Falcon without HDMI\n" +
                "• Refurbishment board",

            "Jasper" =>
                "• Most reliable Fat board\n" +
                "• 65nm CPU + GPU",

            "Tonasket" =>
                "• Jasper v2\n" +
                "• Best Fat motherboard",

            "Trinity" =>
                "• First Slim motherboard\n" +
                "• 45nm XCGPU",

            "Corona" =>
                "• Most common Slim board\n" +
                "• Popular for RGH",

            "Waitsburg" =>
                "• Late Slim revision\n" +
                "• Pre-Stingray",

            "Stingray" =>
                "• First Xbox 360 E board\n" +
                "• Improved power efficiency",

            "Winchester" =>
                "• Final motherboard revision\n" +
                "• Not RGH compatible",

            _ => "No additional information available."
        };
    }

    private static string ResolveLocalPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "";

        if (Path.IsPathRooted(path) && File.Exists(path))
            return path;

        string fromOutput = Path.Combine(AppContext.BaseDirectory, path);

        if (File.Exists(fromOutput))
            return fromOutput;

        string fromProject = Path.Combine(Directory.GetCurrentDirectory(), path);

        if (File.Exists(fromProject))
            return fromProject;

        return path;
    }

    [SlashCommand("motherboards", "Browse all Xbox 360 motherboard revisions.")]
    public async Task MotherboardsAsync()
    {
        var embed = EmbedTheme.Base(
                "🔩 Xbox 360 Motherboard Database",
                "Complete Xbox 360 motherboard revision reference.",
                EmbedTheme.Gold)

            .AddField(
                "📊 Database Statistics",
                $"**Total Revisions:** `{_motherboards.All.Count}`\n" +
                $"**Fat Models:** `7`\n" +
                $"**Slim Models:** `3`\n" +
                $"**E Models:** `2`",
                true)

            .AddField(
                "⚡ Modding Support",
                $"**JTAG Capable:** `Xenon, Elpis, Zephyr, Falcon, Opus, Jasper`\n" +
                $"**RGH Compatible:** `Xenon → Stingray`\n" +
                $"**Not RGH Compatible:** `Winchester`",
                true)

            .AddField(
                "📈 Reliability Overview",
                $"🔴 Poor: `Xenon, Zephyr`\n" +
                $"🟡 Good: `Falcon, Opus, Elpis`\n" +
                $"🟢 Excellent: `Jasper+`",
                true)

            .AddField(
                "📋 Hardware Timeline",
                "```txt\n" +
                "2005  Xenon\n" +
                "2007  Zephyr\n" +
                "2007  Falcon\n" +
                "2008  Opus\n" +
                "2008  Elpis\n" +
                "2008  Jasper\n" +
                "2009  Tonasket\n" +
                "2010  Trinity\n" +
                "2011  Corona\n" +
                "2013  Waitsburg\n" +
                "2013  Stingray\n" +
                "2014  Winchester\n" +
                "```",
                false)

            .AddField(
                "🟥 Xbox 360 Fat",
                "`Xenon`\n`Elpis`\n`Zephyr`\n`Falcon`\n`Opus`\n`Jasper`\n`Tonasket`",
                true)

            .AddField(
                "⬛ Xbox 360 Slim",
                "`Trinity`\n`Corona`\n`Waitsburg`",
                true)

            .AddField(
                "⬜ Xbox 360 E",
                "`Stingray`\n`Winchester`",
                true)

            .AddField(
                "🏆 Most Popular Boards",
                "🥇 `Corona`\n" +
                "🥈 `Trinity`\n" +
                "🥉 `Jasper`",
                true)

            .AddField(
                "🔍 Example Lookups",
                "`/motherboard xenon`\n" +
                "`/motherboard jasper`\n" +
                "`/motherboard tonasket`\n" +
                "`/motherboard corona`\n" +
                "`/motherboard winchester`",
                true)

            .AddField(
                "📚 Quick Reference",
                "• Xenon = Launch motherboard\n" +
                "• Jasper = Most reliable Fat\n" +
                "• Trinity = First Slim\n" +
                "• Corona = Most common Slim\n" +
                "• Winchester = Final revision",
                false)

            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("motherboardcompare", "Compare two Xbox 360 motherboard revisions.")]
    public async Task MotherboardCompareAsync(
    [Summary("board1", "First motherboard, example: Trinity")]
    string board1,
    [Summary("board2", "Second motherboard, example: Corona")]
    string board2)
    {
        var first = _motherboards.Find(board1);
        var second = _motherboards.Find(board2);

        if (first == null || second == null)
        {
            string missing = "";

            if (first == null)
                missing += $"`{board1}` ";

            if (second == null)
                missing += $"`{board2}` ";

            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed(
                    $"Could not find motherboard revision(s): {missing}\n\nTry `Xenon`, `Elpis`, `Zephyr`, `Falcon`, `Opus`, `Jasper`, `Tonasket`, `Trinity`, `Corona`, `Waitsburg`, `Stingray`, or `Winchester`.")
                .Build(),
                ephemeral: true);

            return;
        }

        var embed = EmbedTheme.Base(
                "🔩 Xbox 360 Motherboard Compare",
                $"### `{first.Name}` vs `{second.Name}`",
                EmbedTheme.Gold)

            .AddField(
                "🎮 Console Type",
                $"**{first.Name}:** {EmbedTheme.Empty(first.ConsoleType)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.ConsoleType)}",
                false)

            .AddField(
                "📅 Release Period",
                $"**{first.Name}:** {EmbedTheme.Empty(first.ReleasePeriod)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.ReleasePeriod)}",
                false)

            .AddField(
                "🧠 CPU / GPU",
                $"**{first.Name}:** {EmbedTheme.Empty(first.CpuGpu)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.CpuGpu)}",
                false)

            .AddField(
                "🔌 Power Supply",
                $"**{first.Name}:** {EmbedTheme.Empty(first.PowerSupply)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.PowerSupply)}",
                false)

            .AddField(
                "📺 HDMI",
                $"**{first.Name}:** {EmbedTheme.Empty(first.Hdmi)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.Hdmi)}",
                true)

            .AddField(
                "💾 NAND",
                $"**{first.Name}:** {EmbedTheme.Empty(first.Nand)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.Nand)}",
                true)

            .AddField(
                "⚡ RGH Support",
                $"**{first.Name}:** {EmbedTheme.Empty(first.RghSupport)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.RghSupport)}",
                false)

            .AddField(
                "🔓 JTAG Support",
                $"**{first.Name}:** {EmbedTheme.Empty(first.JtagSupport)}\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.JtagSupport)}",
                true)

            .AddField(
                "📈 Reliability",
                $"**{first.Name}:** {GetReliabilityRating(first.Reliability)}\n" +
                $"**{second.Name}:** {GetReliabilityRating(second.Reliability)}",
                true)

            .AddField(
                "📝 Notes",
                $"**{first.Name}:** {EmbedTheme.Empty(first.Notes)}\n\n" +
                $"**{second.Name}:** {EmbedTheme.Empty(second.Notes)}",
                false)

            .AddField(
                "🧭 Related Commands",
                $"`/motherboard {first.Name}`\n" +
                $"`/motherboard {second.Name}`\n" +
                "`/motherboards`",
                false)

            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("stats", "Displays detailed bot and database statistics.")]
    public async Task StatsAsync()
    {
        int totalDatabaseEntries =
            _hresults.All.Count +
            _titleIds.All.Count +
            _statusCodes.All.Count +
            _dashboards.All.Count +
            _xeBuilds.All.Count +
            _kv.All.Count +
            _challenges.All.Count +
            _motherboards.All.Count;

        double Percent(int value)
        {
            if (totalDatabaseEntries <= 0)
                return 0;

            return Math.Round(
                (double)value / totalDatabaseEntries * 100,
                2);
        }

        string dbStats =
            $"HRESULTs       : {_hresults.All.Count:N0} ({Percent(_hresults.All.Count)}%)\n" +
            $"Title IDs      : {_titleIds.All.Count:N0} ({Percent(_titleIds.All.Count)}%)\n" +
            $"Status Codes   : {_statusCodes.All.Count:N0} ({Percent(_statusCodes.All.Count)}%)\n" +
            $"Dashboards     : {_dashboards.All.Count:N0} ({Percent(_dashboards.All.Count)}%)\n" +
            $"xeBuild        : {_xeBuilds.All.Count:N0} ({Percent(_xeBuilds.All.Count)}%)\n" +
            $"KeyVault       : {_kv.All.Count:N0} ({Percent(_kv.All.Count)}%)\n" +
            $"Challenges     : {_challenges.All.Count:N0} ({Percent(_challenges.All.Count)}%)\n" +
            $"Motherboards   : {_motherboards.All.Count:N0} ({Percent(_motherboards.All.Count)}%)";

        string topCommands = _stats.TopCommands(10).Any()
            ? string.Join(
                "\n",
                _stats.TopCommands(10)
                    .Select((x, i) =>
                        $"`#{i + 1}` /{x.Key} — {x.Value:N0}"))
            : "No command data available.";

        var embed = EmbedTheme.Base(
                "📊 v0id Xbox 360 Statistics Center",
                "Realtime bot analytics and database monitoring.",
                EmbedTheme.Orange)

            .AddField(
                "🤖 Bot Information",
                $"**Servers:** `{Context.Client.Guilds.Count:N0}`\n" +
                $"**Users:** `{Context.Client.Guilds.Sum(x => x.MemberCount):N0}`\n" +
                $"**Slash Commands:** `{_interactionService.SlashCommands.Count:N0}`\n" +
                $"**Uptime:** `{_stats.GetUptime()}`",
                true)

            .AddField(
                "⚡ Usage Statistics",
                $"**Total Interactions:** `{_stats.TotalInteractions:N0}`\n" +
                $"**Slash Commands:** `{_stats.SlashCommandUses:N0}`\n" +
                $"**Components:** `{_stats.ComponentUses:N0}`\n" +
                $"**Modals:** `{_stats.ModalUses:N0}`",
                true)

            .AddField(
                "📦 Database Totals",
                $"**Total Records:** `{totalDatabaseEntries:N0}`\n" +
                $"**Databases:** `8`\n" +
                $"**Largest Database:** `{GetLargestDatabase()}`",
                true)

            .AddField(
                "📚 Realtime Database Breakdown",
                $"```yaml\n{dbStats}\n```",
                false)

            .AddField(
                "🏆 Most Used Commands",
                topCommands,
                false)

            .AddField(
                "🔩 Hardware Database",
                $"**Motherboards:** `{_motherboards.All.Count:N0}`\n" +
                $"**Supported Revisions:** `12`\n" +
                $"**Images Loaded:** `{GetMotherboardImageCount()}`",
                true)

            .AddField(
                "🎮 Xbox Reference Data",
                $"**HRESULTs:** `{_hresults.All.Count:N0}`\n" +
                $"**Title IDs:** `{_titleIds.All.Count:N0}`\n" +
                $"**Challenges:** `{_challenges.All.Count:N0}`",
                true)

            .AddField(
                "🛠 System Health",
                $"🟢 Databases Online\n" +
                $"🟢 Interaction Service Online\n" +
                $"🟢 Discord Gateway Connected\n" +
                $"🟢 Asset System Loaded",
                true)

            .AddField("🧠 Top Database Lookups", BuildDatabaseLookupStats(), false)

            .WithFooter(
                $"v0id Xbox 360 Reference Bot • {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .WithCurrentTimestamp()
            .Build();

        await RespondAsync(embed: embed);
    }

    private string GetLargestDatabase()
    {
        var databases = new Dictionary<string, int>
    {
        { "HRESULTs", _hresults.All.Count },
        { "Title IDs", _titleIds.All.Count },
        { "Status Codes", _statusCodes.All.Count },
        { "Dashboards", _dashboards.All.Count },
        { "xeBuild", _xeBuilds.All.Count },
        { "KeyVault", _kv.All.Count },
        { "Challenges", _challenges.All.Count },
        { "Motherboards", _motherboards.All.Count }
    };

        return databases
            .OrderByDescending(x => x.Value)
            .First()
            .Key;
    }

    private static int GetMotherboardImageCount()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Images",
            "Motherboards");

        if (!Directory.Exists(path))
            return 0;

        return Directory.GetFiles(path, "*.png").Length;
    }

    [SlashCommand("xboxstatus", "Fetch current Xbox LIVE status.")]
    public async Task XboxStatusAsync()
    {
        await DeferAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            using var ws = new ClientWebSocket();

            ws.Options.SetRequestHeader("Origin", "https://xblstatus.com");
            ws.Options.SetRequestHeader("User-Agent", "Mozilla/5.0");
            ws.Options.SetRequestHeader("Pragma", "no-cache");
            ws.Options.SetRequestHeader("Cache-Control", "no-cache");
            ws.Options.SetRequestHeader("Accept-Language", "en-US,en;q=0.9");

            await ws.ConnectAsync(
                new Uri("wss://kvchecker.com/ws/LIVEAuthentication"),
                cts.Token);

            var buffer = new byte[8192];

            while (!cts.Token.IsCancellationRequested)
            {
                var result = await ws.ReceiveAsync(buffer, cts.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("message_type", out var type))
                    continue;

                if (type.GetString() != "xbl_status")
                    continue;

                if (!root.TryGetProperty("services", out var services))
                    continue;

                var sb = new StringBuilder();

                foreach (var service in services.EnumerateArray())
                {
                    string name = service.GetProperty("name").GetString() ?? "Unknown";
                    string description = service.GetProperty("description").GetString() ?? "Unknown";

                    string emoji = description switch
                    {
                        "Fully Operational" => "🟢",
                        "Mostly Operational" => "🟡",
                        "Hardly Operational" => "🟠",
                        "Inoperational" => "🔴",
                        _ => "⚪"
                    };

                    sb.AppendLine($"{emoji} **{name}**: {description}");
                }

                var embed = EmbedTheme.Base(
                        "🌐 Xbox LIVE Status",
                        sb.ToString(),
                        EmbedTheme.Orange)
                    .AddField("📡 Source", "Data provided by `xblstatus.com`.", false)
                    .Build();

                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embed;
                    x.Content = "";
                });

                return;
            }

            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = "No valid Xbox LIVE status data was received within 30 seconds.";
            });
        }
        catch (Exception ex)
        {
            await ModifyOriginalResponseAsync(x =>
            {
                x.Content =
                    "Error connecting to the Xbox LIVE status endpoint.\n" +
                    "This can happen if the endpoint is unavailable or Xbox LIVE status data is delayed.";
            });

            Console.WriteLine($"XboxStatus error: {ex.Message}");
        }
    }

    [SlashCommand("avatarlookup", "Lookup an Xbox Live avatar by gamertag.")]
    public async Task AvatarLookupAsync(
    [Summary("gamertag", "Xbox Live gamertag")]
    string gamertag)
    {
        await DeferAsync();

        gamertag = gamertag.Trim();

        string profilePicUrl =
            $"http://avatar.xboxlive.com/avatar/{Uri.EscapeDataString(gamertag)}/avatarpic-l.png";

        string bodyPicUrl =
            $"http://avatar.xboxlive.com/avatar/{Uri.EscapeDataString(gamertag)}/avatar-body.png";

        try
        {
            using var http = new HttpClient();

            var profileResponse = await http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, profilePicUrl));

            var bodyResponse = await http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, bodyPicUrl));

            if (!profileResponse.IsSuccessStatusCode ||
                !bodyResponse.IsSuccessStatusCode)
            {
                await FollowupAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No Xbox avatar was found for `{gamertag}`.\n\nCheck the spelling and try again.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var embed = EmbedTheme.Base(
                    "🎮 Xbox Avatar Lookup",
                    $"### {gamertag}",
                    EmbedTheme.Primary)
                .AddField(
                    "👤 Gamertag",
                    $"`{gamertag}`",
                    true)
                .AddField(
                    "🖼 Avatar Type",
                    "Xbox 360 Avatar",
                    true)
                .AddField(
                    "🌐 Avatar Service",
                    "Xbox LIVE",
                    true)
                .WithThumbnailUrl(profilePicUrl)
                .WithImageUrl(bodyPicUrl)
                .WithFooter(
                    $"v0id Xbox 360 Reference Bot • Avatar Lookup")
                .WithCurrentTimestamp()
                .Build();

            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = "";
                x.Embed = embed;
            });
        }
        catch
        {
            await ModifyOriginalResponseAsync(x =>
            {
                x.Content =
                    $"Unable to retrieve avatar data for `{gamertag}`.\n" +
                    "The Xbox avatar service may be temporarily unavailable.";
            });
        }
    }

    [SlashCommand("avatardownload", "Show Xbox Live avatar image links by gamertag.")]
    public async Task AvatarDownloadAsync(
    [Summary("gamertag", "Xbox Live gamertag")]
    string gamertag)
    {
        await DeferAsync();

        gamertag = gamertag.Trim();

        string encoded = Uri.EscapeDataString(gamertag);

        string profilePicUrl = $"http://avatar.xboxlive.com/avatar/{encoded}/avatarpic-l.png";
        string bodyPicUrl = $"http://avatar.xboxlive.com/avatar/{encoded}/avatar-body.png";
        string smallPicUrl = $"http://avatar.xboxlive.com/avatar/{encoded}/avatarpic-s.png";

        try
        {
            using var http = new HttpClient();

            var profileResponse = await http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, profilePicUrl));

            var bodyResponse = await http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, bodyPicUrl));

            if (!profileResponse.IsSuccessStatusCode ||
                !bodyResponse.IsSuccessStatusCode)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = EmbedTheme.ErrorEmbed(
                        $"No Xbox avatar was found for `{gamertag}`.\n\nCheck the spelling and try again.")
                        .Build();
                    x.Content = "";
                });

                return;
            }

            var embed = EmbedTheme.Base(
                    "🖼️ Xbox Avatar Download",
                    $"### {gamertag}",
                    EmbedTheme.Primary)
                .AddField("👤 Gamertag", $"`{gamertag}`", true)
                .AddField("🧍 Body Avatar", $"[Open Image]({bodyPicUrl})", true)
                .AddField("🖼️ Large Avatar", $"[Open Image]({profilePicUrl})", true)
                .AddField("🔹 Small Avatar", $"[Open Image]({smallPicUrl})", true)
                .AddField(
                    "📦 Available Assets",
                    "`avatarpic-l.png`\n`avatarpic-s.png`\n`avatar-body.png`",
                    false)
                .WithThumbnailUrl(profilePicUrl)
                .WithImageUrl(bodyPicUrl)
                .WithFooter("v0id Xbox 360 Reference Bot • Avatar Assets")
                .WithCurrentTimestamp()
                .Build();

            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = "";
                x.Embed = embed;
            });
        }
        catch
        {
            await ModifyOriginalResponseAsync(x =>
            {
                x.Content =
                    $"Unable to retrieve avatar assets for `{gamertag}`.\n" +
                    "The Xbox avatar service may be temporarily unavailable.";
            });
        }
    }

    [SlashCommand("gamerpic", "Lookup an Xbox 360 gamerpic by gamertag.")]
    public async Task GamerpicAsync(
    [Summary("gamertag", "Xbox Live gamertag")]
    string gamertag)
    {
        await DeferAsync();

        gamertag = gamertag.Trim();

        string encoded = Uri.EscapeDataString(gamertag);

        string gamerpicUrl =
            $"https://avatar-ssl.xboxlive.com/avatar/{encoded}/avatarpic-l.png";

        try
        {
            using var http = new HttpClient();

            var response = await http.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, gamerpicUrl));

            if (!response.IsSuccessStatusCode)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = EmbedTheme.ErrorEmbed(
                        $"No gamerpic found for `{gamertag}`.")
                        .Build();
                    x.Content = "";
                });

                return;
            }

            var embed = EmbedTheme.Base(
                    "🎮 Xbox Gamerpic Lookup",
                    $"### {gamertag}",
                    EmbedTheme.Primary)
                .AddField("👤 Gamertag", $"`{gamertag}`", true)
                .AddField("🖼️ Image Type", "Xbox Gamerpic", true)
                .AddField("📥 Direct Link", $"[Open Gamerpic]({gamerpicUrl})", true)
                .WithThumbnailUrl(gamerpicUrl)
                .WithImageUrl(gamerpicUrl)
                .Build();

            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = "";
                x.Embed = embed;
            });
        }
        catch
        {
            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Unable to retrieve gamerpic for `{gamertag}`.";
            });
        }
    }

    [SlashCommand("recommend", "Get recommended Xbox 360 hardware/setup reference info.")]
    public async Task RecommendAsync(string query)
    {
        _stats.TrackDatabaseLookup("Recommendations");

        var entry = _recommend.Find(query);

        if (entry == null)
        {
            var results = _recommend.Search(query, 10);

            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No recommendation found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var embedSearch = EmbedTheme.SearchEmbed("⭐ Recommendation Search Results", results.Count);

            foreach (var result in results)
                embedSearch.AddField(result.Name, $"**Category:** {result.Category}\n**For:** {result.RecommendedFor}", false);

            await RespondAsync(embed: embedSearch.Build());
            return;
        }

        var embed = EmbedTheme.Base("⭐ Recommendation Reference", $"### {entry.Name}", EmbedTheme.Gold)
            .AddField("📁 Category", SafeField(entry.Category), true)
            .AddField("🎯 Recommended For", SafeField(entry.RecommendedFor), true)
            .AddField("✅ Recommendation", SafeField(entry.Recommendation), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("dvddrive", "Look up Xbox 360 DVD drive reference info.")]
    public async Task DvdDriveAsync(string query)
    {
        _stats.TrackDatabaseLookup("DVD Drives");

        var entry = _dvdDrives.Find(query);

        if (entry == null)
        {
            var results = _dvdDrives.Search(query, 10);

            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No DVD drive found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var embedSearch = EmbedTheme.SearchEmbed("💿 DVD Drive Search Results", results.Count);

            foreach (var result in results)
                embedSearch.AddField(result.Name, $"**Manufacturer:** {result.Manufacturer}\n**Models:** {result.Models}\n**Console:** {result.ConsoleType}", false);

            await RespondAsync(embed: embedSearch.Build());
            return;
        }

        var embed = EmbedTheme.Base("💿 Xbox 360 DVD Drive", $"### {entry.Name}", EmbedTheme.Orange)
            .AddField("🏷️ Manufacturer", SafeField(entry.Manufacturer), true)
            .AddField("🔢 Models", SafeField(entry.Models), true)
            .AddField("🎮 Console Type", SafeField(entry.ConsoleType), true)
            .AddField("🔑 Key Info", SafeField(entry.KeyInfo), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("xblports", "Show Xbox LIVE network ports.")]
    public async Task XblPortsAsync()
    {
        _stats.TrackDatabaseLookup("Xbox LIVE Ports");

        var embed = EmbedTheme.Base(
            "🌐 Xbox LIVE Network Ports",
            "Required Xbox LIVE port reference for Xbox 360 connectivity.",
            EmbedTheme.Cyan);

        foreach (var port in _xblPorts.All)
        {
            embed.AddField(
                $"{port.Protocol} {port.Port}",
                $"**Service:** {SafeField(port.Service)}\n**Direction:** {SafeField(port.Direction)}\n**Notes:** {SafeField(port.Notes)}",
                false);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("nand", "Look up Xbox 360 NAND reference info.")]
    public async Task NandAsync(string query)
    {
        _stats.TrackDatabaseLookup("NAND");

        var entry = _nand.Find(query);

        if (entry == null)
        {
            var results = _nand.Search(query, 10);

            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No NAND entry found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("💾 NAND Search Results", results.Count);

            foreach (var result in results)
                search.AddField(result.Name, $"**Boards:** {result.Boards}\n**Layout:** {result.Layout}\n**Size:** {result.Size}", false);

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base("💾 Xbox 360 NAND Reference", $"### {entry.Name}", EmbedTheme.Gold)
            .AddField("🔩 Boards", SafeField(entry.Boards), false)
            .AddField("🏭 Manufacturer", SafeField(entry.Manufacturer), true)
            .AddField("📦 Size", SafeField(entry.Size), true)
            .AddField("🧱 Layout", SafeField(entry.Layout), true)
            .AddField("🔢 Block Count", SafeField(entry.BlockCount), true)
            .AddField("🧬 ECC", SafeField(entry.Ecc), true)
            .AddField("⚠️ Bad Blocks", SafeField(entry.BadBlockHandling), false)
            .AddField("🛠️ Recommended Tools", SafeField(entry.RecommendedTools), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("tu", "Look up Xbox 360 title update info.")]
    public async Task TitleUpdateAsync(string query)
    {
        _stats.TrackDatabaseLookup("Title Updates");

        var results = _titleUpdates.Search(query, 10);

        if (results.Count == 0)
        {
            await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No title updates found for `{query}`.").Build(), ephemeral: true);
            return;
        }

        var embed = EmbedTheme.Base("📦 Title Update Reference", $"Results for `{query}`", EmbedTheme.Blue);

        foreach (var tu in results)
        {
            embed.AddField(
                $"{tu.Game} — {tu.Update}",
                $"**Short Name:** `{tu.ShortName}`\n**Title ID:** `{tu.TitleId}`\n**Media ID:** `{SafeField(tu.MediaId)}`\n**Dashboard:** {SafeField(tu.Dashboard)}\n**Notes:** {SafeField(tu.Notes)}",
                false);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("xex", "Look up Xbox 360 XEX reference info.")]
    public async Task XexAsync(string query)
    {
        _stats.TrackDatabaseLookup("XEX");

        var entry = _xex.Find(query);

        if (entry == null)
        {
            var results = _xex.Search(query, 10);

            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No XEX entry found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var embedSearch = EmbedTheme.SearchEmbed("🧩 XEX Search Results", results.Count);

            foreach (var result in results)
                embedSearch.AddField(result.Name, $"**Game:** {result.Game}\n**Title ID:** `{result.TitleId}`", false);

            await RespondAsync(embed: embedSearch.Build());
            return;
        }

        var embed = EmbedTheme.Base("🧩 XEX Reference", $"### {entry.Name}", EmbedTheme.Orange)
            .AddField("🎮 Game", SafeField(entry.Game), true)
            .AddField("🆔 Title ID", $"`{SafeField(entry.TitleId)}`", true)
            .AddField("💿 Media ID", $"`{SafeField(entry.MediaId)}`", true)
            .AddField("⚙️ Flags", SafeField(entry.Flags), false)
            .AddField("📚 Libraries", SafeField(entry.Libraries), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("console", "Look up Xbox 360 console model information.")]
    public async Task ConsoleAsync(string query)
    {
        _stats.TrackDatabaseLookup("Consoles");

        var entry = _consoles.Find(query);

        if (entry == null)
        {
            var results = _consoles.Search(query, 10);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed($"No console model found for `{query}`.").Build(),
                    ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("🎮 Console Search Results", results.Count);

            foreach (var result in results)
            {
                search.AddField(
                    result.Name,
                    $"**Family:** {result.Family}\n**Storage:** {result.Storage}\n**Boards:** {result.Motherboards}",
                    false);
            }

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base(
                "🎮 Xbox 360 Console Reference",
                $"### {entry.Name}\n`{entry.Family}`",
                EmbedTheme.Primary)
            .AddField("📅 Release Date", SafeField(entry.ReleaseDate), true)
            .AddField("📦 Family", SafeField(entry.Family), true)
            .AddField("📺 HDMI", SafeField(entry.Hdmi), true)
            .AddField("🔩 Motherboards", SafeField(entry.Motherboards), false)
            .AddField("💾 Storage", SafeField(entry.Storage), false)
            .AddField("🔌 Power Supply", SafeField(entry.PowerSupply), false)
            .AddField("🎨 Finish", SafeField(entry.Finish), true)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("consoles", "Browse Xbox 360 console families.")]
    public async Task ConsolesAsync()
    {
        _stats.TrackDatabaseLookup("Consoles");

        var embed = EmbedTheme.Base(
            "🎮 Xbox 360 Console Families",
            $"Current console database entries: `{_consoles.All.Count:N0}`",
            EmbedTheme.Primary);

        foreach (var group in _consoles.All.GroupBy(x => x.Family).OrderBy(x => x.Key))
        {
            string names = string.Join("\n", group.Select(x => $"`{x.Name}`"));

            embed.AddField(
                $"📦 {group.Key}",
                names,
                true);
        }

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("powersupply", "Look up Xbox 360 power supply compatibility.")]
    public async Task PowerSupplyAsync(string query)
    {
        var entry = _powerSupplies.Find(query);

        if (entry == null)
        {
            var results = _powerSupplies.Search(query, 10);
            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No power supply entry found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("🔌 Power Supply Results", results.Count);
            foreach (var result in results)
                search.AddField(result.Name, $"**Wattage:** {result.Wattage}\n**Boards:** {result.CompatibleBoards}", false);

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base("🔌 Xbox 360 Power Supply", $"### {entry.Name}", EmbedTheme.Gold)
            .AddField("⚡ Wattage", SafeField(entry.Wattage), true)
            .AddField("🔩 Compatible Boards", SafeField(entry.CompatibleBoards), false)
            .AddField("🔗 Connector", SafeField(entry.ConnectorType), true)
            .AddField("📈 Voltage", SafeField(entry.Voltage), true)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("southbridge", "Look up Xbox 360 southbridge/ANA/HANA reference info.")]
    public async Task SouthbridgeAsync(string query)
    {
        var entry = _southbridges.Find(query);

        if (entry == null)
        {
            await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No southbridge entry found for `{query}`.").Build(), ephemeral: true);
            return;
        }

        var embed = EmbedTheme.Base("🧠 Xbox 360 Southbridge Reference", $"### {entry.Name}", EmbedTheme.Orange)
            .AddField("📦 Board Family", SafeField(entry.BoardFamily), true)
            .AddField("🔩 Used On", SafeField(entry.UsedOn), true)
            .AddField("⚙️ Role", SafeField(entry.Role), false)
            .AddField("⚠️ Common Issues", SafeField(entry.CommonIssues), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("ram", "Look up Xbox 360 RAM package reference info.")]
    public async Task RamAsync(string query)
    {
        var results = _ram.Search(query, 10);

        if (results.Count == 0)
        {
            await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No RAM entry found for `{query}`.").Build(), ephemeral: true);
            return;
        }

        var embed = EmbedTheme.Base("🧬 Xbox 360 RAM Reference", $"Results for `{query}`", EmbedTheme.Cyan);

        foreach (var entry in results)
            embed.AddField(entry.Manufacturer, $"**Type:** {entry.Type}\n**Used On:** {entry.UsedOn}\n**Capacity:** {entry.Capacity}\n**Notes:** {entry.Notes}", false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("gpu", "Look up Xbox 360 GPU/XCGPU reference info.")]
    public async Task GpuAsync(string query)
    {
        var entry = _gpu.Find(query);

        if (entry == null)
        {
            var results = _gpu.Search(query, 10);
            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No GPU entry found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("🧩 GPU Search Results", results.Count);
            foreach (var result in results)
                search.AddField($"{result.Name} {result.Revision}", $"**Process:** {result.Process}\n**Used On:** {result.UsedOn}", false);

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base("🧩 Xbox 360 GPU Reference", $"### {entry.Name}", EmbedTheme.Primary)
            .AddField("🏷️ Revision", SafeField(entry.Revision), true)
            .AddField("📐 Process", SafeField(entry.Process), true)
            .AddField("🔩 Used On", SafeField(entry.UsedOn), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("achievement", "Look up Xbox 360 achievement set information.")]
    public async Task AchievementAsync(string query)
    {
        var entry = _achievements.Find(query);

        if (entry == null)
        {
            var results = _achievements.Search(query, 10);
            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No achievement entry found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("🏆 Achievement Search Results", results.Count);
            foreach (var result in results)
                search.AddField(result.Game, $"**Title ID:** `{result.TitleId}`\n**Gamerscore:** {result.TotalGamerscore}", false);

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base("🏆 Xbox 360 Achievement Reference", $"### {entry.Game}", EmbedTheme.Gold)
            .AddField("🆔 Title ID", $"`{SafeField(entry.TitleId)}`", true)
            .AddField("🎯 Achievements", SafeField(entry.AchievementCount), true)
            .AddField("💯 Gamerscore", SafeField(entry.TotalGamerscore), true)
            .AddField("❔ Secret", SafeField(entry.SecretAchievements), true)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("cert", "Look up Xbox 360 certificate reference information.")]
    public async Task CertAsync(string query)
    {
        var entry = _certs.Find(query);

        if (entry == null)
        {
            await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No certificate entry found for `{query}`.").Build(), ephemeral: true);
            return;
        }

        var embed = EmbedTheme.Base("📜 Xbox 360 Certificate Reference", $"### {entry.Name}", EmbedTheme.Cyan)
            .AddField("📁 Area", SafeField(entry.Area), true)
            .AddField("🧠 Meaning", SafeField(entry.Meaning), false)
            .AddField("✅ Validation Use", SafeField(entry.ValidationUse), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("xexflag", "Look up Xbox 360 XEX flag information.")]
    public async Task XexFlagAsync(string query)
    {
        var entry = _xexFlags.Find(query);

        if (entry == null)
        {
            var results = _xexFlags.Search(query, 10);
            if (results.Count == 0)
            {
                await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No XEX flag entry found for `{query}`.").Build(), ephemeral: true);
                return;
            }

            var search = EmbedTheme.SearchEmbed("📦 XEX Flag Search Results", results.Count);
            foreach (var result in results)
                search.AddField(result.Name, $"**Category:** {result.Category}\n**Meaning:** {result.Meaning}", false);

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base("📦 XEX Flag Reference", $"### {entry.Name}", EmbedTheme.Primary)
            .AddField("📁 Category", SafeField(entry.Category), true)
            .AddField("🧠 Meaning", SafeField(entry.Meaning), false)
            .AddField("⚙️ Common Use", SafeField(entry.CommonUse), false)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("dvdfirmware", "Look up Xbox 360 DVD firmware reference information.")]
    public async Task DvdFirmwareAsync(string query)
    {
        var results = _dvdFirmware.Search(query, 10);

        if (results.Count == 0)
        {
            await RespondAsync(embed: EmbedTheme.ErrorEmbed($"No DVD firmware entry found for `{query}`.").Build(), ephemeral: true);
            return;
        }

        var embed = EmbedTheme.Base("💿 Xbox 360 DVD Firmware Reference", $"Results for `{query}`", EmbedTheme.Orange);

        foreach (var entry in results)
            embed.AddField(entry.Drive, $"**Firmware:** {entry.Firmware}\n**Console:** {entry.ConsoleType}\n**Key Storage:** {entry.KeyStorage}\n**Notes:** {entry.Notes}", false);

        await RespondAsync(embed: embed.Build());
    }

    private string BuildDatabaseLookupStats()
    {
        var top = _stats.TopDatabaseLookups(10);

        if (top.Count == 0)
            return "No database lookup data recorded yet.";

        return string.Join("\n", top.Select((x, i) => $"`#{i + 1}` **{x.Key}** — `{x.Value:N0}` lookups"));
    }

    private string BuildDatabaseHealthStats()
    {
        return
            $"**HRESULTs:** `{_hresults.All.Count:N0}`\n" +
            $"**Title IDs:** `{_titleIds.All.Count:N0}`\n" +
            $"**Status Codes:** `{_statusCodes.All.Count:N0}`\n" +
            $"**Dashboards:** `{_dashboards.All.Count:N0}`\n" +
            $"**xeBuild:** `{_xeBuilds.All.Count:N0}`\n" +
            $"**KeyVault:** `{_kv.All.Count:N0}`\n" +
            $"**Challenges:** `{_challenges.All.Count:N0}`\n" +
            $"**Motherboards:** `{_motherboards.All.Count:N0}`";
    }

    [SlashCommand("encyclopedia", "Search across all Xbox 360 reference databases.")]
    public async Task EncyclopediaAsync(string query)
    {
        var embed = EmbedTheme.Base(
                "📚 Xbox 360 Encyclopedia",
                $"Global reference search for `{query}`",
                EmbedTheme.Orange);

        var hresult = _hresults.Search(query).FirstOrDefault();
        if (hresult != null)
            embed.AddField("🧩 HRESULT", $"`{hresult.Code}` — **{hresult.Name}**\n{SafeField(hresult.Meaning)}", false);

        var title = _titleIds.Search(query).FirstOrDefault();
        if (title != null)
            embed.AddField("🎮 Title ID", $"**{title.Game}**\n`{title.TitleId}`\n{SafeField(title.Notes)}", false);

        var motherboard = _motherboards.Search(query).FirstOrDefault();
        if (motherboard != null)
            embed.AddField("🔩 Motherboard", $"**{motherboard.Name}**\n{motherboard.ConsoleType}\n{motherboard.CpuGpu}", false);

        var dashboard = _dashboards.Search(query).FirstOrDefault();
        if (dashboard != null)
            embed.AddField("🖥️ Dashboard", $"**{dashboard.Version}**\n{dashboard.Description}", false);

        var kernel = _kernels.Search(query).FirstOrDefault();
        if (kernel != null)
            embed.AddField("🧠 Kernel", $"**{kernel.Version}**\n{kernel.Codename}\n{kernel.Description}", false);

        var kv = _kv.Search(query).FirstOrDefault();
        if (kv != null)
            embed.AddField("🔑 KeyVault", $"**{kv.Key}**\n{kv.Meaning}", false);

        var challenge = _challenges.Search(query).FirstOrDefault();
        if (challenge != null)
            embed.AddField("🛡️ Challenge", $"**{challenge.Name}**\n{challenge.Area}\n{challenge.Meaning}", false);

        var xex = _xex.Search(query).FirstOrDefault();
        if (xex != null)
            embed.AddField("📦 XEX", $"**{xex.Name}**\n{xex.Game}\n`{xex.TitleId}`", false);

        var tu = _titleUpdates.Search(query).FirstOrDefault();
        if (tu != null)
            embed.AddField("📥 Title Update", $"**{tu.Game}** — `{tu.Update}`\nTitle ID: `{tu.TitleId}`", false);

        var achievement = _achievements.Search(query).FirstOrDefault();
        if (achievement != null)
            embed.AddField("🏆 Achievements", $"**{achievement.Game}**\n{achievement.AchievementCount} achievements • {achievement.TotalGamerscore}", false);

        if (embed.Fields.Count == 0)
        {
            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed($"No encyclopedia results found for `{query}`.").Build(),
                ephemeral: true);
            return;
        }

        embed.AddField(
            "🔍 More Detail",
            $"Use focused commands like `/titleid {query}`, `/lookup {query}`, `/motherboard {query}`, `/xex {query}`, `/tu {query}`.",
            false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("servers", "Show all servers the bot is currently in.")]
    public async Task ServersAsync()
    {
        var guilds = Context.Client.Guilds
            .OrderByDescending(x => x.MemberCount)
            .ToList();

        int totalUsers = guilds.Sum(x => x.MemberCount);

        var embed = EmbedTheme.Base(
                "🌐 v0id Network Overview",
                $"Connected to **{guilds.Count:N0}** servers.",
                EmbedTheme.Orange)

            .AddField(
                "📊 Global Statistics",
                $"**Servers:** `{guilds.Count:N0}`\n" +
                $"**Users:** `{totalUsers:N0}`\n" +
                $"**Average Size:** `{(guilds.Count > 0 ? totalUsers / guilds.Count : 0):N0}`",
                false);

        int rank = 1;

        foreach (var guild in guilds.Take(20))
        {
            embed.AddField(
                $"#{rank++} {guild.Name}",
                $"🆔 `{guild.Id}`\n" +
                $"👥 `{guild.MemberCount:N0}` Members\n" +
                $"👑 Owner: <@{guild.OwnerId}>",
                true);
        }

        embed
            .WithFooter($"v0id Xbox 360 Reference Bot • {guilds.Count:N0} Connected Servers")
            .WithCurrentTimestamp();

        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }

    [SlashCommand("repair", "Xbox 360 repair assistant.")]
    public async Task RepairAsync(string issue)
    {
        _stats.TrackDatabaseLookup("Repair Assistant");

        var entry = _repairs.Find(issue);

        if (entry == null)
        {
            var results = _repairs.Search(issue, 10);

            if (results.Count == 0)
            {
                await RespondAsync(
                    embed: EmbedTheme.ErrorEmbed(
                        $"No repair entry found for `{issue}`.\n\nTry `E74`, `E79`, `0102`, `0022`, `RROD`, `No Video`, `Open Tray`, `Bad NAND`, or `No Power`.")
                    .Build(),
                    ephemeral: true);

                return;
            }

            var search = EmbedTheme.SearchEmbed("🛠️ Repair Search Results", results.Count);

            foreach (var result in results)
            {
                search.AddField(
                    $"🛠️ {result.Issue}",
                    $"**Category:** {SafeField(result.Category)}\n" +
                    $"**Difficulty:** {SafeField(result.Difficulty)}\n" +
                    $"**Meaning:** {Shorten(SafeField(result.Meaning), 160)}",
                    false);
            }

            await RespondAsync(embed: search.Build());
            return;
        }

        var embed = EmbedTheme.Base(
                "🛠️ Xbox 360 Repair Assistant",
                $"### {entry.Issue}\n`{entry.Category}`",
                EmbedTheme.Orange)

            .AddField("📁 Category", SafeField(entry.Category), true)
            .AddField("📊 Difficulty", SafeField(entry.Difficulty), true)
            .AddField("⚠️ Risk Level", SafeField(entry.RiskLevel), true)
            .AddField("🧠 Meaning", SafeField(entry.Meaning), false)
            .AddField("🔩 Common Boards", SafeField(entry.CommonBoards), false)
            .AddField("🧪 Likely Causes", SafeField(entry.LikelyCauses), false)
            .AddField("🧰 Tools Needed", SafeField(entry.ToolsNeeded), false)
            .AddField("✅ Recommended Steps", SafeField(entry.RecommendedSteps), false)
            .AddField("🛡️ Prevention", SafeField(entry.Prevention), false)
            .AddField("🧭 Related Codes", SafeField(entry.RelatedCodes), true)
            .AddField("📝 Notes", SafeField(entry.Notes), false)
            .WithFooter("v0id Xbox 360 Reference Bot • Repair Assistant")
            .WithCurrentTimestamp()
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("repairlist", "Browse Xbox 360 repair assistant issues.")]
    public async Task RepairListAsync()
    {
        _stats.TrackDatabaseLookup("Repair Assistant");

        var groups = _repairs.All
            .GroupBy(x => x.Category)
            .OrderBy(x => x.Key)
            .ToList();

        var embed = EmbedTheme.Base(
                "🛠️ Xbox 360 Repair Database",
                $"Current repair entries: `{_repairs.All.Count:N0}`",
                EmbedTheme.Orange);

        foreach (var group in groups)
        {
            string issues = string.Join(
                "\n",
                group.Select(x => $"`{x.Issue}`").Take(12));

            embed.AddField(
                $"📁 {group.Key}",
                issues,
                true);
        }

        embed.AddField(
            "🔍 Examples",
            "`/repair e74`\n`/repair e79`\n`/repair 0102`\n`/repair rrod`\n`/repair no video`",
            false);

        await RespondAsync(embed: embed.Build());
    }

    [SlashCommand("repaircategory", "Browse repair entries by category.")]
    public async Task RepairCategoryAsync(string category)
    {
        _stats.TrackDatabaseLookup("Repair Assistant");

        var results = _repairs.ByCategory(category, 15);

        if (results.Count == 0)
        {
            await RespondAsync(
                embed: EmbedTheme.ErrorEmbed($"No repair category found for `{category}`.").Build(),
                ephemeral: true);

            return;
        }

        var embed = EmbedTheme.Base(
                $"🛠️ Repair Category: {category}",
                $"Showing `{results.Count:N0}` repair issue(s).",
                EmbedTheme.Orange);

        foreach (var result in results)
        {
            embed.AddField(
                result.Issue,
                $"**Difficulty:** {SafeField(result.Difficulty)}\n" +
                $"**Cause:** {Shorten(SafeField(result.LikelyCauses), 180)}",
                false);
        }

        await RespondAsync(embed: embed.Build());
    }

}