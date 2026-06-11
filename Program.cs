using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using XboxHresultBot.Database;
using XboxHresultBot.Modules;

namespace XboxHresultBot;

internal static class Program
{
    private const string ConfigFile = "appsettings.json";

    private static DiscordSocketClient _client = null!;
    private static InteractionService _interactions = null!;
    private static IServiceProvider _services = null!;
    private static BotConfig _config = null!;

    private static readonly DateTime StartedAt = DateTime.Now;
    private static bool _commandsRegistered;

    public static async Task Main()
    {
        Console.Title = "v0id Xbox 360 Reference Bot • WAR ROOM";
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.CursorVisible = false;
        PrintBootScreen();
        EnsureFolders();

        _config = BotConfig.Load(ConfigFile);

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds,
            AlwaysDownloadUsers = false,
            MessageCacheSize = 0,
            LogLevel = LogSeverity.Info
        });

        _interactions = new InteractionService(_client.Rest, new InteractionServiceConfig
        {
            LogLevel = LogSeverity.Info,
            DefaultRunMode = RunMode.Async,
            ThrowOnError = false
        });

        _services = BuildServices();

        HookEvents();

        WriteLog("CONFIG", $"Loaded {ConfigFile}", ConsoleColor.Green);
        WriteLog("SYSTEM", "Initializing v0id command core...", ConsoleColor.Cyan);
        WriteLog("GATEWAY", "Connecting to Discord gateway...", ConsoleColor.Cyan);

        await _client.LoginAsync(TokenType.Bot, _config.Token);
        await _client.StartAsync();

        _ = Task.Run(ActivityLoopAsync);

        PrintRuntimeHelp();

        await Task.Delay(-1);
    }

    private static ServiceProvider BuildServices()
    {
        return new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_interactions)

            .AddSingleton(new HresultDatabase("hresults.json"))
            .AddSingleton(new TitleIdDatabase("titleids.json"))
            .AddSingleton(new StatusCodeDatabase("statuscodes.json"))
            .AddSingleton(new DashboardDatabase("dashboards.json"))
            .AddSingleton(new XeBuildDatabase("xebuild.json"))
            .AddSingleton(new KVDatabase("kv.json"))
            .AddSingleton(new ChallengeDatabase("challenges.json"))
            .AddSingleton(new MotherboardDatabase("motherboards.json"))

            .AddSingleton(new RecommendDatabase())
            .AddSingleton(new DvdDriveDatabase())
            .AddSingleton(new XblPortDatabase())
            .AddSingleton(new NandDatabase())
            .AddSingleton(new TitleUpdateDatabase())
            .AddSingleton(new XexDatabase())
            .AddSingleton(new KernelDatabase())
            .AddSingleton(new ConsoleDatabase())
            .AddSingleton(new PowerSupplyDatabase())
            .AddSingleton(new SouthbridgeDatabase())
            .AddSingleton(new RamDatabase())
            .AddSingleton(new GpuDatabase())
            .AddSingleton(new AchievementDatabase())
            .AddSingleton(new CertDatabase())
            .AddSingleton(new XexFlagDatabase())
            .AddSingleton(new DvdFirmwareDatabase())
            .AddSingleton(new RepairDatabase())

            .AddSingleton(new BotStatistics("botstats.json"))
            .BuildServiceProvider();
    }

    private static void HookEvents()
    {
        _client.Log += LogAsync;
        _interactions.Log += LogAsync;

        _client.Ready += ReadyAsync;
        _client.InteractionCreated += InteractionCreatedAsync;

        _client.Connected += () =>
        {
            WriteLog("GATEWAY", "Connected", ConsoleColor.Green);
            return Task.CompletedTask;
        };

        _client.Disconnected += ex =>
        {
            WriteLog("GATEWAY", $"Disconnected, reconnecting automatically: {ex.Message}", ConsoleColor.Yellow);
            return Task.CompletedTask;
        };

        _client.JoinedGuild += guild =>
        {
            WriteLog("GUILD", $"Joined {guild.Name} ({guild.Id})", ConsoleColor.Green);
            PrintControlPanel();
            return Task.CompletedTask;
        };

        _client.LeftGuild += guild =>
        {
            WriteLog("GUILD", $"Left {guild.Name} ({guild.Id})", ConsoleColor.Yellow);
            PrintControlPanel();
            return Task.CompletedTask;
        };

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            _ = ShutdownAsync();
        };

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            WriteLog("FATAL", e.ExceptionObject?.ToString() ?? "Unknown fatal error", ConsoleColor.DarkRed);
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            WriteLog("TASK", e.Exception.ToString(), ConsoleColor.Red);
            e.SetObserved();
        };
    }

    private static async Task ReadyAsync()
    {
        WriteLog(
            "READY",
            $"Logged in as {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}",
            ConsoleColor.Green);

        if (_commandsRegistered)
        {
            WriteLog("READY", "Duplicate ready event ignored", ConsoleColor.DarkGray);
            return;
        }

        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        if (_config.GuildId > 0)
        {
            await _interactions.RegisterCommandsToGuildAsync(_config.GuildId, true);
            WriteLog("COMMANDS", $"Registered to guild {_config.GuildId}", ConsoleColor.Green);
        }
        else
        {
            await _interactions.RegisterCommandsGloballyAsync(true);
            WriteLog("COMMANDS", "Registered globally", ConsoleColor.Green);
        }

        _commandsRegistered = true;

        ValidateDatabases();
        ValidateAssets();
        PrintControlPanel();
    }

    private static async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        var stats = _services.GetRequiredService<BotStatistics>();
        string name = GetInteractionName(interaction);

        try
        {
            switch (interaction)
            {
                case SocketSlashCommand slash:
                    stats.TrackSlashCommand(slash.CommandName);
                    break;

                case SocketMessageComponent:
                    stats.TrackComponent();
                    break;

                case SocketModal:
                    stats.TrackModal();
                    break;
            }

            WriteLog("COMMAND", $"{interaction.User.Username} used {name}", ConsoleColor.Yellow);
            AddRecentActivity($"{interaction.User.Username} used {name}");

            var context = new SocketInteractionContext(_client, interaction);
            var result = await _interactions.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                WriteLog("FAILED", $"{name}: {result.ErrorReason}", ConsoleColor.Red);

                if (!interaction.HasResponded)
                {
                    await interaction.RespondAsync(
                        $"Command failed: `{result.ErrorReason}`",
                        ephemeral: true);
                }
            }

            PrintLiveLine();
        }
        catch (Exception ex)
        {
            WriteLog("ERROR", ex.ToString(), ConsoleColor.Red);

            try
            {
                if (interaction.HasResponded)
                {
                    await interaction.FollowupAsync(
                        "An error occurred while running this command.",
                        ephemeral: true);
                }
                else
                {
                    await interaction.RespondAsync(
                        "An error occurred while running this command.",
                        ephemeral: true);
                }
            }
            catch
            {
                WriteLog("ERROR", "Failed to send Discord error response", ConsoleColor.Red);
            }
        }
    }

    private static void AddRecentActivity(string message)
    {
        string line = $"[{DateTime.Now:HH:mm:ss}] {message}";

        RecentActivity.Enqueue(line);

        while (RecentActivity.Count > 6)
            RecentActivity.Dequeue();
    }

    private static async Task ActivityLoopAsync()
    {
        string[] activities =
        {
            "HRESULT intelligence",
            "Xbox 360 hardware database",
            "KV reference systems",
            "XEX metadata archive",
            "dashboard timelines",
            "motherboard diagnostics",
            "v0id Network control"
        };

        int index = 0;

        while (true)
        {
            try
            {
                string activity = activities[index++ % activities.Length];

                await _client.SetGameAsync(
                    activity,
                    type: ActivityType.Watching);

                await Task.Delay(TimeSpan.FromMinutes(3));
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }

    private static Task LogAsync(LogMessage message)
    {
        if (message.Exception is System.Net.WebSockets.WebSocketException &&
            message.Exception.Message.Contains("closed", StringComparison.OrdinalIgnoreCase))
        {
            WriteLog("GATEWAY", "Discord websocket closed; reconnecting automatically", ConsoleColor.Yellow);
            return Task.CompletedTask;
        }

        ConsoleColor color = message.Severity switch
        {
            LogSeverity.Critical => ConsoleColor.DarkRed,
            LogSeverity.Error => ConsoleColor.Red,
            LogSeverity.Warning => ConsoleColor.Yellow,
            LogSeverity.Info => ConsoleColor.Gray,
            LogSeverity.Verbose => ConsoleColor.DarkGray,
            LogSeverity.Debug => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        WriteLog(
            message.Severity.ToString().ToUpperInvariant(),
            $"{message.Source}: {message.Message}",
            color);

        if (message.Exception != null)
            WriteLog("EXCEPTION", message.Exception.Message, ConsoleColor.Red);

        return Task.CompletedTask;
    }

    private static async Task ShutdownAsync()
    {
        WriteLog("SYSTEM", "Shutdown requested...", ConsoleColor.Yellow);

        try
        {
            await _client.SetStatusAsync(UserStatus.Offline);
            await _client.StopAsync();
            await _client.LogoutAsync();

            WriteLog("SYSTEM", "Bot stopped safely", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            WriteLog("ERROR", ex.Message, ConsoleColor.Red);
        }

        Console.CursorVisible = true;

        Environment.Exit(0);
    }

    private static string GetInteractionName(SocketInteraction interaction)
    {
        return interaction switch
        {
            SocketSlashCommand slash => $"/{slash.CommandName}",
            SocketMessageComponent component => $"component:{component.Data.CustomId}",
            SocketModal modal => $"modal:{modal.Data.CustomId}",
            _ => interaction.Type.ToString()
        };
    }

    private static void EnsureFolders()
    {
        string[] folders =
        {
            "Data",
            "Images",
            "Images/Consoles",
            "Images/Motherboards",
            "Logs"
        };

        foreach (string folder in folders)
            Directory.CreateDirectory(folder);

        WriteLog("FILES", "Required folders checked", ConsoleColor.Green);
    }

    private static void ValidateDatabases()
    {
        PrintMiniHeader("DATABASE CHECK");

        PrintDatabase("HRESULTs", _services.GetRequiredService<HresultDatabase>().All.Count);
        PrintDatabase("Title IDs", _services.GetRequiredService<TitleIdDatabase>().All.Count);
        PrintDatabase("Status Codes", _services.GetRequiredService<StatusCodeDatabase>().All.Count);
        PrintDatabase("Dashboards", _services.GetRequiredService<DashboardDatabase>().All.Count);
        PrintDatabase("xeBuild", _services.GetRequiredService<XeBuildDatabase>().All.Count);
        PrintDatabase("KeyVault", _services.GetRequiredService<KVDatabase>().All.Count);
        PrintDatabase("Challenges", _services.GetRequiredService<ChallengeDatabase>().All.Count);
        PrintDatabase("Motherboards", _services.GetRequiredService<MotherboardDatabase>().All.Count);
        PrintDatabase("Kernels", _services.GetRequiredService<KernelDatabase>().All.Count);
        PrintDatabase("Consoles", _services.GetRequiredService<ConsoleDatabase>().All.Count);
        PrintDatabase("DVD Drives", _services.GetRequiredService<DvdDriveDatabase>().All.Count);
        PrintDatabase("XEX", _services.GetRequiredService<XexDatabase>().All.Count);
        PrintDatabase("Achievements", _services.GetRequiredService<AchievementDatabase>().All.Count);
    }

    private static void PrintDatabase(string name, int count)
    {
        WriteLog(
            count > 0 ? "ONLINE" : "EMPTY",
            $"{name}: {count:N0} entries",
            count > 0 ? ConsoleColor.Green : ConsoleColor.Yellow);
    }

    private static void ValidateAssets()
    {
        string[] images =
        {
            "Images/Consoles/fat.png",
            "Images/Consoles/slim.png",
            "Images/Consoles/e.png",

            "Images/Motherboards/xenon.png",
            "Images/Motherboards/elpis.png",
            "Images/Motherboards/zephyr.png",
            "Images/Motherboards/falcon.png",
            "Images/Motherboards/opus.png",
            "Images/Motherboards/jasper.png",
            "Images/Motherboards/tonasket.png",
            "Images/Motherboards/trinity.png",
            "Images/Motherboards/corona.png",
            "Images/Motherboards/waitsburg.png",
            "Images/Motherboards/stingray.png",
            "Images/Motherboards/winchester.png"
        };

        int found = images.Count(File.Exists);
        int missing = images.Length - found;

        WriteLog(
            "ASSETS",
            $"Images found: {found:N0}, missing: {missing:N0}",
            missing == 0 ? ConsoleColor.Green : ConsoleColor.Yellow);
    }

    private static void PrintBootScreen()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("""
        ╔════════════════════════════════════════════════════════════════════════════╗
        ║                                                                            ║
        ║       ██╗   ██╗ ██████╗ ██╗██████╗     ██████╗  ██████╗ ████████╗          ║
        ║       ██║   ██║██╔═████╗██║██╔══██╗    ██╔══██╗██╔═══██╗╚══██╔══╝          ║
        ║       ██║   ██║██║██╔██║██║██║  ██║    ██████╔╝██║   ██║   ██║             ║
        ║       ╚██╗ ██╔╝████╔╝██║██║██║  ██║    ██╔══██╗██║   ██║   ██║             ║
        ║        ╚████╔╝ ╚██████╔╝██║██████╔╝    ██████╔╝╚██████╔╝   ██║             ║
        ║         ╚═══╝   ╚═════╝ ╚═╝╚═════╝     ╚═════╝  ╚═════╝    ╚═╝             ║
        ║                                                                            ║
        ║                 XBOX 360 REFERENCE WAR ROOM                                ║
        ║                                                                            ║
        ║    HRESULTS • TITLE IDS • KERNELS • HARDWARE • KV • XEX • LIVE             ║
        ║    CONTROL CENTER • DATABASES • DIAGNOSTICS • ANALYTICS                    ║
        ║                                                                            ║
        ╚════════════════════════════════════════════════════════════════════════════╝
        """);
        Console.ResetColor();

        Console.WriteLine();
    }

    private static void PrintRuntimeHelp()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Runtime:");
        Console.WriteLine("  CTRL + C  - Stop bot safely");
        Console.WriteLine("  Global slash commands may take time to update across Discord.");
        Console.WriteLine();
        Console.ResetColor();
    }

    private static readonly Queue<string> RecentActivity = new();

    private static void PrintControlPanel()
    {
        var stats = _services.GetRequiredService<BotStatistics>();
        var db = GetDatabaseStats();
        int total = db.Sum(x => x.Count);

        Console.Clear();
        PrintBootScreen();

        DashboardHeader("XBOX 360 REFERENCE DASHBOARD");

        DrawDashboardRow(
            "SYSTEM",
            new[]
            {
            $"Status        ONLINE",
            $"Guilds        {_client.Guilds.Count:N0}",
            $"Users         {_client.Guilds.Sum(x => x.MemberCount):N0}",
            $"Commands      {_interactions.SlashCommands.Count:N0}",
            $"Uptime        {GetUptime()}"
            },
            "DATABASES",
            db.OrderByDescending(x => x.Count)
                .Take(8)
                .Select(x => $"{x.Name.PadRight(14)} {x.Count:N0}")
                .ToArray()
        );

        DrawTilePanel(
            "COMMAND ACTIVITY",
            stats.TopCommands(6).Count == 0
                ? new[] { "No command usage recorded yet." }
                : stats.TopCommands(6)
                    .Select(x => $"/{x.Key}".PadRight(24) + $"{x.Value:N0} uses")
                    .ToArray());

        DrawTilePanel(
            "SYSTEM HEALTH",
            new[]
            {
            "Xbox Live Gateway       READY",
            "Interaction Service     ONLINE",
            "Database Core           ONLINE",
            "Asset Loader            ONLINE",
            "Command Router          ONLINE",
            "Owner Core              ARMED"
            });

        DrawTilePanel(
            "DATABASE BREAKDOWN",
            db.OrderByDescending(x => x.Count)
                .Take(8)
                .Select(x => BuildXboxBar(x.Name, x.Count, total))
                .ToArray());

        DrawTilePanel(
            "RECENT ACTIVITY",
            RecentActivity.Count == 0
                ? new[] { "No recent activity yet." }
                : RecentActivity.ToArray());
    }

    private static List<DatabaseStat> GetDatabaseStats()
    {
        return new()
    {
        new DatabaseStat(
            "HRESULTs",
            _services.GetRequiredService<HresultDatabase>().All.Count),

        new DatabaseStat(
            "Title IDs",
            _services.GetRequiredService<TitleIdDatabase>().All.Count),

        new DatabaseStat(
            "Status Codes",
            _services.GetRequiredService<StatusCodeDatabase>().All.Count),

        new DatabaseStat(
            "Dashboards",
            _services.GetRequiredService<DashboardDatabase>().All.Count),

        new DatabaseStat(
            "xeBuild",
            _services.GetRequiredService<XeBuildDatabase>().All.Count),

        new DatabaseStat(
            "KeyVault",
            _services.GetRequiredService<KVDatabase>().All.Count),

        new DatabaseStat(
            "Challenges",
            _services.GetRequiredService<ChallengeDatabase>().All.Count),

        new DatabaseStat(
            "Motherboards",
            _services.GetRequiredService<MotherboardDatabase>().All.Count),

        new DatabaseStat(
            "Kernels",
            _services.GetRequiredService<KernelDatabase>().All.Count),

        new DatabaseStat(
            "Consoles",
            _services.GetRequiredService<ConsoleDatabase>().All.Count),

        new DatabaseStat(
            "DVD Drives",
            _services.GetRequiredService<DvdDriveDatabase>().All.Count),

        new DatabaseStat(
            "NAND",
            _services.GetRequiredService<NandDatabase>().All.Count),

        new DatabaseStat(
            "Title Updates",
            _services.GetRequiredService<TitleUpdateDatabase>().All.Count),

        new DatabaseStat(
            "XEX",
            _services.GetRequiredService<XexDatabase>().All.Count),

        new DatabaseStat(
            "Achievements",
            _services.GetRequiredService<AchievementDatabase>().All.Count),

        new DatabaseStat(
            "Power Supplies",
            _services.GetRequiredService<PowerSupplyDatabase>().All.Count),

        new DatabaseStat(
            "Southbridges",
            _services.GetRequiredService<SouthbridgeDatabase>().All.Count),

        new DatabaseStat(
            "GPUs",
            _services.GetRequiredService<GpuDatabase>().All.Count),

        new DatabaseStat(
            "RAM",
            _services.GetRequiredService<RamDatabase>().All.Count),

        new DatabaseStat(
            "Certificates",
            _services.GetRequiredService<CertDatabase>().All.Count),

        new DatabaseStat(
            "XEX Flags",
            _services.GetRequiredService<XexFlagDatabase>().All.Count),

        new DatabaseStat(
            "DVD Firmware",
            _services.GetRequiredService<DvdFirmwareDatabase>().All.Count)
    };
    }

    private sealed record DatabaseStat(
    string Name,
    int Count);

    private static void DashboardHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║ {title.PadRight(76)} ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void DrawDashboardRow(
        string leftTitle,
        string[] leftLines,
        string rightTitle,
        string[] rightLines)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"┌─ {leftTitle.PadRight(31, '─')}┐ ┌─ {rightTitle.PadRight(39, '─')}┐");
        Console.ResetColor();

        int max = Math.Max(leftLines.Length, rightLines.Length);

        for (int i = 0; i < max; i++)
        {
            string left = i < leftLines.Length ? leftLines[i] : "";
            string right = i < rightLines.Length ? rightLines[i] : "";

            Console.Write("│ ");
            WriteXboxText(left.PadRight(31));
            Console.Write(" │ │ ");
            WriteXboxText(right.PadRight(39));
            Console.WriteLine(" │");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("└─────────────────────────────────┘ └─────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void DrawTilePanel(string title, string[] lines)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"┌─ {title.PadRight(72, '─')}┐");
        Console.ResetColor();

        foreach (string line in lines.Take(10))
        {
            Console.Write("│ ");
            WriteXboxText(line.PadRight(72));
            Console.WriteLine(" │");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("└──────────────────────────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void WriteXboxText(string text)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(text);
        Console.ResetColor();
    }

    private static string BuildXboxBar(string name, int count, int total)
    {
        double percent = total <= 0 ? 0 : (double)count / total * 100;
        int filled = Math.Clamp((int)Math.Round(percent / 5), 0, 20);

        string bar = new string('█', filled) + new string('░', 20 - filled);
        string percentText = percent < 1 && count > 0 ? "<1%" : $"{percent:0}%";

        return $"{name.PadRight(15)} {bar} {percentText.PadLeft(4)}";
    }

    private static void DrawDualStatusPanel(
    int hresults,
    int titleIds,
    int statusCodes,
    int dashboards,
    int xeBuilds,
    int kv,
    int challenges,
    int motherboards)
    {
        string[] left =
        {
        Row("Status", "Online", 30),
        Row("Guilds", _client.Guilds.Count.ToString("N0"), 30),
        Row("Users", _client.Guilds.Sum(x => x.MemberCount).ToString("N0"), 30),
        Row("Commands", _interactions.SlashCommands.Count.ToString("N0"), 30),
        Row("Uptime", GetUptime(), 30)
    };

        string[] right =
        {
        Row("HRESULTs", hresults.ToString("N0"), 47),
        Row("Title IDs", titleIds.ToString("N0"), 47),
        Row("Status Codes", statusCodes.ToString("N0"), 47),
        Row("Dashboards", dashboards.ToString("N0"), 47),
        Row("xeBuild", xeBuilds.ToString("N0"), 47),
        Row("KeyVault", kv.ToString("N0"), 47),
        Row("Challenges", challenges.ToString("N0"), 47),
        Row("Motherboards", motherboards.ToString("N0"), 47)
    };

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("┌─ BOT STATUS ─────────────────────┐ ┌─ DATABASE STACK ──────────────────────────────┐");
        Console.ResetColor();

        int max = Math.Max(left.Length, right.Length);

        for (int i = 0; i < max; i++)
        {
            string l = i < left.Length ? $"│ {left[i]} │" : "└──────────────────────────────────┘";
            string r = i < right.Length ? $"│ {right[i]} │" : "└───────────────────────────────────────────────┘";

            Console.WriteLine($"{l} {r}");
        }

        Console.WriteLine();
    }

    private static void DrawCommandAnalyticsPanel(BotStatistics stats)
    {
        PanelWideTop("COMMAND ANALYTICS");

        var top = stats.TopCommands(5);

        if (top.Count == 0)
        {
            WideLine("No command usage recorded yet.");
        }
        else
        {
            foreach (var command in top)
                WideLine($"/{command.Key}".PadRight(24) + $"{command.Value:N0} uses");
        }

        PanelWideBottom();
        Console.WriteLine();
    }

    private static void DrawHealthPanel()
    {
        PanelWideTop("LIVE SYSTEM HEALTH");

        WideLine("Discord Gateway      ● ONLINE");
        WideLine("Interaction Service  ● ONLINE");
        WideLine("Database Engine      ● ONLINE");
        WideLine("Asset Loader         ● ONLINE");
        WideLine("Xbox Status API      ● READY");
        WideLine("Command Router       ● ONLINE");
        WideLine("Owner Core           ● ARMED");

        PanelWideBottom();
        Console.WriteLine();
    }

    private static void DrawDatabaseBreakdownPanel(
    int total,
    int hresults,
    int titleIds,
    int challenges,
    int kv,
    int motherboards)
    {
        PanelWideTop("DATABASE DOMINANCE");

        WideLine(BuildBar("HRESULTs", hresults, total));
        WideLine(BuildBar("Title IDs", titleIds, total));
        WideLine(BuildBar("Challenges", challenges, total));
        WideLine(BuildBar("KeyVault", kv, total));
        WideLine(BuildBar("Motherboards", motherboards, total));

        PanelWideBottom();
        Console.WriteLine();
    }

    private static void DrawRecentActivityPanel()
    {
        PanelWideTop("RECENT ACTIVITY");

        if (RecentActivity.Count == 0)
        {
            WideLine("No recent activity yet.");
        }
        else
        {
            foreach (string line in RecentActivity)
                WideLine(line);
        }

        PanelWideBottom();
        Console.WriteLine();
    }

    private static string Row(string label, string value, int width)
    {
        string text = $"{label.PadRight(14)} {value}";

        return text.Length > width
            ? text[..width]
            : text.PadRight(width);
    }

    private static void PanelWideTop(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"┌─ {title} " + new string('─', Math.Max(0, 80 - title.Length - 3)) + "┐");
        Console.ResetColor();
    }

    private static void PanelWideBottom()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("└────────────────────────────────────────────────────────────────────────────────────┘");
        Console.ResetColor();
    }

    private static void WideLine(string text)
    {
        if (text.Length > 82)
            text = text[..82];

        Console.WriteLine($"│ {text.PadRight(82)} │");
    }

    private static string BuildBar(string name, int count, int total)
    {
        double percent = total <= 0 ? 0 : (double)count / total * 100;
        int filled = total <= 0 ? 0 : (int)Math.Round(percent / 5);
        filled = Math.Clamp(filled, 0, 20);

        string bar = new string('█', filled) + new string('░', 20 - filled);
        string percentText = percent < 1 && count > 0 ? "<1%" : $"{percent:0}%";

        return $"{name.PadRight(13)} {bar} {percentText}";
    }

    private static void PrintTopCommands(BotStatistics stats)
    {
        var top = stats.TopCommands(8);

        if (top.Count == 0)
            return;

        PanelTop("TOP COMMANDS");

        foreach (var command in top)
            Stat($"/{command.Key}", $"{command.Value:N0} uses");

        PanelBottom();
    }

    private static void PrintLiveLine()
    {
        var stats = _services.GetRequiredService<BotStatistics>();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("           ");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("LIVE ");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("| Guilds ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(_client.Guilds.Count);

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(" | Uses ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(stats.TotalInteractions);

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(" | Today ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(stats.UsesToday());

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(" | Uptime ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(GetUptime());

        Console.ResetColor();
    }

    private static void PrintMiniHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine();
        Console.WriteLine($"── {title} ───────────────────────────────────────────────");
        Console.ResetColor();
    }

    private static void PanelTop(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║ {title.PadRight(68)} ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════════════╣");
        Console.ResetColor();
    }

    private static void PanelMid()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("╠══════════════════════════════════════════════════════════════════════╣");
        Console.ResetColor();
    }

    private static void PanelBottom()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void Stat(string name, string value)
    {
        const int width = 70;

        string left = $" {name}";
        string right = value;

        if (right.Length > 38)
            right = right[..35] + "...";

        int spaces = width - left.Length - right.Length;

        if (spaces < 1)
            spaces = 1;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("║");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(left);

        Console.Write(new string(' ', spaces));

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(right);

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("║");

        Console.ResetColor();
    }

    private static void WriteLog(string tag, string message, ConsoleColor color)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] ");

        Console.ForegroundColor = color;
        Console.Write($"[{tag.PadRight(9)}] ");

        Console.ResetColor();
        Console.WriteLine(message);

        WriteFileLog(tag, message);
    }

    private static void WriteFileLog(string tag, string message)
    {
        try
        {
            Directory.CreateDirectory("Logs");
            File.AppendAllText(
                Path.Combine("Logs", $"{DateTime.Now:yyyy-MM-dd}.log"),
                $"[{DateTime.Now:HH:mm:ss}] [{tag}] {message}{Environment.NewLine}");
        }
        catch
        {
            // Never crash the bot because logging failed.
        }
    }

    private static string GetUptime()
    {
        TimeSpan uptime = DateTime.Now - StartedAt;

        if (uptime.TotalDays >= 1)
            return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";

        if (uptime.TotalHours >= 1)
            return $"{uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";

        if (uptime.TotalMinutes >= 1)
            return $"{uptime.Minutes}m {uptime.Seconds}s";

        return $"{uptime.Seconds}s";
    }
}

public sealed class BotConfig
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string Token { get; set; } = "";
    public ulong GuildId { get; set; }

    public static BotConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            var example = new BotConfig
            {
                Token = "PUT_YOUR_BOT_TOKEN_HERE",
                GuildId = 0
            };

            File.WriteAllText(path, JsonSerializer.Serialize(example, JsonOptions));

            throw new FileNotFoundException($"{path} was created. Add your token and restart.");
        }

        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<BotConfig>(json)
               ?? throw new InvalidOperationException($"Invalid {path}.");
    }
}