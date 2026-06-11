using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class AchievementDatabase
{
    private readonly List<AchievementEntry> _entries = new()
    {
        new()
        {
            Game = "Call of Duty 4: Modern Warfare",
            ShortName = "cod4",
            TitleId = "415607E6",
            AchievementCount = "37",
            TotalGamerscore = "1000G",
            SecretAchievements = "0",
            Notes = "Retail achievement set reference."
        },
        new()
        {
            Game = "Call of Duty: Modern Warfare 2",
            ShortName = "mw2",
            TitleId = "41560817",
            AchievementCount = "50",
            TotalGamerscore = "1000G",
            SecretAchievements = "0",
            Notes = "Retail achievement set reference."
        },
        new()
        {
            Game = "Call of Duty: Black Ops II",
            ShortName = "bo2",
            TitleId = "415608C3",
            AchievementCount = "50",
            TotalGamerscore = "1000G",
            SecretAchievements = "Multiple",
            Notes = "Retail achievement set reference."
        },
        new()
        {
            Game = "Grand Theft Auto V",
            ShortName = "gta5",
            TitleId = "545408A7",
            AchievementCount = "49",
            TotalGamerscore = "1000G",
            SecretAchievements = "Multiple",
            Notes = "Retail achievement set reference."
        },
    };

    public IReadOnlyList<AchievementEntry> All => _entries;

    public AchievementEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x => Normalize(x.Game) == q || Normalize(x.ShortName) == q || Normalize(x.TitleId) == q || Normalize(x.AchievementCount) == q);
    }

    public List<AchievementEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x => x.Game.Contains(query, StringComparison.OrdinalIgnoreCase) || x.ShortName.Contains(query, StringComparison.OrdinalIgnoreCase) || x.TitleId.Contains(query, StringComparison.OrdinalIgnoreCase) || x.AchievementCount.Contains(query, StringComparison.OrdinalIgnoreCase) || x.TotalGamerscore.Contains(query, StringComparison.OrdinalIgnoreCase) || x.SecretAchievements.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Notes.Contains(query, StringComparison.OrdinalIgnoreCase))
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
