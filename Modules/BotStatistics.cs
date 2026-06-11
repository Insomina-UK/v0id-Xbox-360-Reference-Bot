using Discord;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XboxHresultBot.Modules;

public sealed class BotStatistics
{
    private readonly string _path;
    private readonly object _lock = new();

    public DateTime StartedAt { get; private set; } = DateTime.Now;

    public long TotalInteractions { get; private set; }
    public long SlashCommandUses { get; private set; }
    public long ComponentUses { get; private set; }
    public long ModalUses { get; private set; }

    public Dictionary<string, long> CommandUses { get; private set; } = new();
    public Dictionary<string, long> DailyUses { get; private set; } = new();
    public Dictionary<string, long> DatabaseLookups { get; private set; } = new();

    public BotStatistics(string path)
    {
        _path = path;
        Load();
        StartedAt = DateTime.Now;
    }

    public void TrackSlashCommand(string commandName)
    {
        lock (_lock)
        {
            TotalInteractions++;
            SlashCommandUses++;

            string clean = Normalize(commandName);
            Increment(CommandUses, clean);
            Increment(DailyUses, DateTime.Now.ToString("yyyy-MM-dd"));

            Save();
        }
    }

    public void TrackComponent()
    {
        lock (_lock)
        {
            TotalInteractions++;
            ComponentUses++;
            Increment(DailyUses, DateTime.Now.ToString("yyyy-MM-dd"));
            Save();
        }
    }

    public void TrackModal()
    {
        lock (_lock)
        {
            TotalInteractions++;
            ModalUses++;
            Increment(DailyUses, DateTime.Now.ToString("yyyy-MM-dd"));
            Save();
        }
    }

    public void TrackDatabaseLookup(string databaseName)
    {
        lock (_lock)
        {
            Increment(DatabaseLookups, databaseName);
            Save();
        }
    }

    public long GetCommandUses(string commandName)
    {
        string clean = Normalize(commandName);

        lock (_lock)
        {
            return CommandUses.TryGetValue(clean, out long value)
                ? value
                : 0;
        }
    }

    public string GetUptime()
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

    public List<KeyValuePair<string, long>> TopCommands(int count = 10)
    {
        lock (_lock)
        {
            return CommandUses
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .Take(count)
                .ToList();
        }
    }

    public List<KeyValuePair<string, long>> TopDatabaseLookups(int count = 10)
    {
        lock (_lock)
        {
            return DatabaseLookups
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .Take(count)
                .ToList();
        }
    }

    public long UsesToday()
    {
        string key = DateTime.Now.ToString("yyyy-MM-dd");

        lock (_lock)
        {
            return DailyUses.TryGetValue(key, out long value)
                ? value
                : 0;
        }
    }

    public long UsesLast7Days()
    {
        lock (_lock)
        {
            long total = 0;

            for (int i = 0; i < 7; i++)
            {
                string key = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");

                if (DailyUses.TryGetValue(key, out long value))
                    total += value;
            }

            return total;
        }
    }

    public double AverageUsesPerDay()
    {
        lock (_lock)
        {
            if (DailyUses.Count == 0)
                return 0;

            return Math.Round(DailyUses.Values.Average(), 2);
        }
    }

    private void Load()
    {
        if (!File.Exists(_path))
        {
            Save();
            return;
        }

        string json = File.ReadAllText(_path);

        var data = JsonSerializer.Deserialize<BotStatisticsData>(json);

        if (data == null)
            return;

        TotalInteractions = data.TotalInteractions;
        SlashCommandUses = data.SlashCommandUses;
        ComponentUses = data.ComponentUses;
        ModalUses = data.ModalUses;
        CommandUses = data.CommandUses ?? new();
        DailyUses = data.DailyUses ?? new();
        DatabaseLookups = data.DatabaseLookups ?? new();
    }

    private void Save()
    {
        var data = new BotStatisticsData
        {
            TotalInteractions = TotalInteractions,
            SlashCommandUses = SlashCommandUses,
            ComponentUses = ComponentUses,
            ModalUses = ModalUses,
            CommandUses = CommandUses,
            DailyUses = DailyUses,
            DatabaseLookups = DatabaseLookups
        };

        File.WriteAllText(_path, JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private static void Increment(Dictionary<string, long> dictionary, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            key = "unknown";

        if (!dictionary.ContainsKey(key))
            dictionary[key] = 0;

        dictionary[key]++;
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .TrimStart('/')
            .ToLowerInvariant();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private sealed class BotStatisticsData
    {
        public long TotalInteractions { get; set; }
        public long SlashCommandUses { get; set; }
        public long ComponentUses { get; set; }
        public long ModalUses { get; set; }

        public Dictionary<string, long> CommandUses { get; set; } = new();
        public Dictionary<string, long> DailyUses { get; set; } = new();
        public Dictionary<string, long> DatabaseLookups { get; set; } = new();
    }
}