using Discord;

namespace XboxHresultBot.Theme;

public static class EmbedTheme
{
    // ─────────────────────────────────────────────────────────────
    // v0id Brand Colors
    // ─────────────────────────────────────────────────────────────

    public static readonly Color Orange = new(238, 93, 14);
    public static readonly Color DarkOrange = new(190, 55, 5);
    public static readonly Color Gold = new(245, 158, 11);
    public static readonly Color Red = new(239, 68, 68);
    public static readonly Color DarkRed = new(127, 29, 29);
    public static readonly Color Green = new(34, 197, 94);
    public static readonly Color Blue = new(59, 130, 246);
    public static readonly Color Cyan = new(34, 211, 238);
    public static readonly Color Purple = new(168, 85, 247);
    public static readonly Color Pink = new(236, 72, 153);
    public static readonly Color Gray = new(107, 114, 128);
    public static readonly Color Dark = new(17, 24, 39);

    public static readonly Color Primary = Orange;
    public static readonly Color Success = Green;
    public static readonly Color Warning = Gold;
    public static readonly Color Error = Red;
    public static readonly Color Info = Cyan;

    public const string BrandName = "v0id Xbox 360 Reference Bot";
    public const string BrandIconUrl = "";
    public const string DefaultThumbnailUrl = "";
    public const string Separator = "━━━━━━━━━━━━━━━━━━━━━━";

    // ─────────────────────────────────────────────────────────────
    // Core Builders
    // ─────────────────────────────────────────────────────────────

    public static EmbedBuilder Base(string title, string description, Color? color = null)
    {
        return new EmbedBuilder()
            .WithColor(color ?? Orange)
            .WithTitle($"⛧ {title}")
            .WithDescription(
                $"```ansi\n" +
                $"\u001b[1;38;5;202m{description}\u001b[0m\n" +
                $"```")
            .WithFooter("v0id Network • Xbox 360 Reference Core")
            .WithCurrentTimestamp();
    }

    public static EmbedBuilder Premium(
        string title,
        string description,
        Color? color = null)
    {
        return Base(
                $"⛧ {title}",
                "```ansi\n" +
                "\u001b[1;38;5;202mSYSTEM ONLINE • REFERENCE CORE ACTIVE\u001b[0m\n" +
                "\u001b[0;37m" + StripCodeBlock(description) + "\u001b[0m\n" +
                "```",
                color ?? Orange)
            .AddField("⚡ Status", "`ONLINE`", true)
            .AddField("🧠 Core", "`v0id`", true)
            .AddField("📡 Mode", "`Reference`", true);
    }

    public static EmbedBuilder ControlPanel(
        string title,
        string subtitle,
        Color? color = null)
    {
        return Base(
                title,
                $"```ansi\n\u001b[1;38;5;202m{EscapeAnsi(subtitle)}\u001b[0m\n```",
                color ?? Orange)
            .AddField("🧭 Navigation", "Use slash commands to browse the database.", false);
    }

    // ─────────────────────────────────────────────────────────────
    // Common Embed Types
    // ─────────────────────────────────────────────────────────────

    public static EmbedBuilder ErrorEmbed(string message)
    {
        return Base(
                "❌ Error",
                SafeDescription(message),
                Error)
            .AddField("🛠️ Fix", "Check your input and try again.", false);
    }

    public static EmbedBuilder SuccessEmbed(string message)
    {
        return Base(
            "✅ Success",
            SafeDescription(message),
            Success);
    }

    public static EmbedBuilder WarningEmbed(string message)
    {
        return Base(
            "⚠️ Warning",
            SafeDescription(message),
            Warning);
    }

    public static EmbedBuilder InfoEmbed(string message)
    {
        return Base(
            "ℹ️ Information",
            SafeDescription(message),
            Info);
    }

    public static EmbedBuilder SearchEmbed(string title, int resultCount)
    {
        return Base(
                title,
                $"Found `{resultCount:N0}` result(s).",
                Cyan)
            .AddField("🔎 Search Mode", "`Smart Reference Lookup`", true)
            .AddField("📦 Results", $"`{resultCount:N0}`", true);
    }

    public static EmbedBuilder EmptyEmbed(string title, string query)
    {
        return Base(
                "🔍 No Results",
                $"No results were found for `{Empty(query)}`.",
                Gray)
            .AddField("💡 Tip", "Try a shorter query, an ID, a category, or an alias.", false);
    }

    // ─────────────────────────────────────────────────────────────
    // Xbox 360 Specific Cards
    // ─────────────────────────────────────────────────────────────

    public static EmbedBuilder HresultCard(
        string code,
        string name,
        string category,
        string meaning,
        string cause,
        string fix)
    {
        return Base(
                "🧩 HRESULT Intelligence Report",
                $"### `{Empty(code)}`\n**{Empty(name)}**",
                Orange)
            .AddField("📁 Category", Empty(category), true)
            .AddField("🏷️ Symbol", $"`{Empty(name)}`", true)
            .AddField("🧠 Meaning", Empty(meaning), false)
            .AddField("⚠️ Common Cause", Empty(cause), false)
            .AddField("🛠️ Suggested Fix", Empty(fix), false)
            .AddField("🧭 Related", $"`/search {Empty(category)}`\n`/list`", false);
    }

    public static EmbedBuilder DatabaseCard(
        string title,
        string description,
        int count,
        Color? color = null)
    {
        return Base(
                title,
                description,
                color ?? Orange)
            .AddField("📦 Entries", $"`{count:N0}`", true)
            .AddField("📡 Status", count > 0 ? "`ONLINE`" : "`EMPTY`", true)
            .AddField("🧠 Engine", "`v0id Database Core`", true);
    }

    public static EmbedBuilder HardwareCard(
        string title,
        string description)
    {
        return Base(
                $"🔩 {title}",
                description,
                Gold)
            .AddField("📁 Type", "`Hardware Reference`", true)
            .AddField("🛠️ Usage", "`Diagnostic / Identification`", true);
    }

    public static EmbedBuilder KvCard(
        string title,
        string description)
    {
        return Base(
                $"🔑 {title}",
                description,
                Red)
            .AddField("📁 Type", "`KeyVault Reference`", true)
            .AddField("🛡️ Notice", "Reference-only documentation.", false);
    }

    public static EmbedBuilder XexCard(
        string title,
        string description)
    {
        return Base(
                $"📦 {title}",
                description,
                Purple)
            .AddField("📁 Type", "`XEX Metadata`", true)
            .AddField("🧩 Module", "`Executable Reference`", true);
    }

    public static EmbedBuilder XboxLiveCard(
        string title,
        string description)
    {
        return Base(
                $"🌐 {title}",
                description,
                Cyan)
            .AddField("📡 Source", "`Xbox LIVE Reference`", true)
            .AddField("⚡ Status", "`Live Data / Cached Reference`", true);
    }

    // ─────────────────────────────────────────────────────────────
    // Advanced Panels
    // ─────────────────────────────────────────────────────────────

    public static EmbedBuilder StatsPanel(
        string title,
        string description,
        IEnumerable<(string Name, string Value, bool Inline)> fields)
    {
        var embed = Base(title, description, Orange);

        foreach (var field in fields)
        {
            embed.AddField(
                SafeFieldName(field.Name),
                Empty(field.Value),
                field.Inline);
        }

        return embed;
    }

    public static EmbedBuilder CommandCenter(
        string title,
        string mode,
        string status)
    {
        return Base(
                title,
                "```ansi\n" +
                "\u001b[1;38;5;202mV0ID CONTROL LAYER ONLINE\u001b[0m\n" +
                "\u001b[0;37m" + EscapeAnsi(mode) + "\u001b[0m\n" +
                "```",
                Orange)
            .AddField("📡 Status", $"`{Empty(status)}`", true)
            .AddField("🧠 Mode", $"`{Empty(mode)}`", true)
            .AddField("🔒 Access", "`Authorized`", true);
    }

    public static EmbedBuilder HelpCenter()
    {
        return Base(
            "📘 v0id Help Center",
            "Xbox 360 reference, diagnostics, databases, and tools.",
            Orange);
    }

    // ─────────────────────────────────────────────────────────────
    // Field Helpers
    // ─────────────────────────────────────────────────────────────

    public static string Empty(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "Not available"
            : value.Trim();
    }

    public static string Code(string? value)
    {
        return $"`{Empty(value)}`";
    }

    public static string Bold(string? value)
    {
        return $"**{Empty(value)}**";
    }

    public static string Bullet(params string?[] values)
    {
        var items = values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => $"• {x!.Trim()}")
            .ToList();

        return items.Count == 0
            ? "Not available"
            : string.Join("\n", items);
    }

    public static string InlineList(IEnumerable<string?> values)
    {
        var items = values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => $"`{x!.Trim()}`")
            .ToList();

        return items.Count == 0
            ? "Not available"
            : string.Join(" ", items);
    }

    public static string Truncate(string? value, int maxLength = 1024)
    {
        value = Empty(value);

        if (value.Length <= maxLength)
            return value;

        return value[..Math.Max(0, maxLength - 3)] + "...";
    }

    public static string ProgressBar(int value, int total, int size = 12)
    {
        if (total <= 0)
            return new string('░', size) + " 0%";

        double percent = Math.Clamp((double)value / total, 0, 1);
        int filled = (int)Math.Round(percent * size);

        string bar = new string('█', filled) + new string('░', size - filled);

        return $"{bar} {percent * 100:0}%";
    }

    public static string StatusEmoji(bool online)
    {
        return online ? "🟢" : "🔴";
    }

    public static string StatusText(bool online)
    {
        return online ? "ONLINE" : "OFFLINE";
    }

    public static string YesNo(bool value)
    {
        return value ? "✅ Yes" : "❌ No";
    }

    // ─────────────────────────────────────────────────────────────
    // Builder Extensions
    // ─────────────────────────────────────────────────────────────

    public static EmbedBuilder AddSafeField(
        this EmbedBuilder embed,
        string name,
        string? value,
        bool inline = false)
    {
        embed.AddField(
            SafeFieldName(name),
            Truncate(value),
            inline);

        return embed;
    }

    public static EmbedBuilder AddCodeField(
        this EmbedBuilder embed,
        string name,
        string? value,
        bool inline = false)
    {
        embed.AddField(
            SafeFieldName(name),
            $"`{Empty(value)}`",
            inline);

        return embed;
    }

    public static EmbedBuilder AddSection(
        this EmbedBuilder embed,
        string title,
        string? value)
    {
        embed.AddField(
            $"▸ {SafeFieldName(title)}",
            Truncate(value),
            false);

        return embed;
    }

    public static EmbedBuilder WithBrandFooter(this EmbedBuilder embed)
    {
        embed.WithFooter(BrandName);
        embed.WithCurrentTimestamp();
        return embed;
    }

    public static EmbedBuilder WithDatabaseFooter(this EmbedBuilder embed, string database, int count)
    {
        embed.WithFooter($"{BrandName} • {database} • {count:N0} entries");
        embed.WithCurrentTimestamp();
        return embed;
    }

    public static EmbedBuilder WithWarningFooter(this EmbedBuilder embed)
    {
        embed.WithFooter($"{BrandName} • Reference-only documentation");
        embed.WithCurrentTimestamp();
        return embed;
    }

    // ─────────────────────────────────────────────────────────────
    // Safety
    // ─────────────────────────────────────────────────────────────

    private static string SafeTitle(string? title)
    {
        title = Empty(title);

        return title.Length <= 256
            ? title
            : title[..253] + "...";
    }

    private static string SafeDescription(string? description)
    {
        description = Empty(description);

        return description.Length <= 4096
            ? description
            : description[..4093] + "...";
    }

    private static string SafeFieldName(string? name)
    {
        name = Empty(name);

        return name.Length <= 256
            ? name
            : name[..253] + "...";
    }

    private static string StripCodeBlock(string value)
    {
        return value
            .Replace("```", "")
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();
    }

    private static string EscapeAnsi(string value)
    {
        return value
            .Replace("\u001b", "")
            .Replace("```", "")
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();
    }
}
