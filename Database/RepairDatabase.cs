using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class RepairDatabase
{
    private readonly List<RepairEntry> _entries = new()
    {
        new()
        {
            Issue = "E74",
            Aliases = "e74, hana, ana, video error, scaler error",
            Category = "Video / Hardware",
            Meaning = "General hardware/video output fault commonly associated with ANA/HANA, GPU, or AV output path issues.",
            CommonBoards = "Xenon, Zephyr, Falcon, Jasper",
            LikelyCauses = "ANA/HANA fault, GPU fault, damaged AV/HDMI path, cracked solder joints, overheating history.",
            Difficulty = "Medium / Advanced",
            RiskLevel = "High if rework is attempted without proper equipment.",
            ToolsNeeded = "Torx tools, known-good AV/HDMI cable, known-good display, multimeter, inspection microscope, proper rework station if board repair is required.",
            RecommendedSteps = "1. Test a known-good AV/HDMI cable and display.\\n2. Remove storage devices and retest.\\n3. Inspect HDMI/AV port for physical damage.\\n4. Check for overheating or previous repair damage.\\n5. If the fault remains, inspect ANA/HANA/GPU area.\\n6. Avoid heat-gun fixes; use proper diagnostics and rework equipment.",
            Prevention = "Keep console clean, ventilated, and avoid overheating. Do not flex the motherboard.",
            RelatedCodes = "E74, 1022, 0022, no video",
            Notes = "Reference-only repair guidance. Board-level work should only be attempted with proper tools and experience.",
        },
        new()
        {
            Issue = "E79",
            Aliases = "e79, dashboard error, nand error, bad update, hdd error",
            Category = "Boot / NAND / Storage",
            Meaning = "Boot failure often linked to dashboard files, NAND image issues, bad system update, HDD problems, or filesystem corruption.",
            CommonBoards = "All Xbox 360 revisions",
            LikelyCauses = "Corrupted NAND image, failed update, bad HDD, corrupted system files, damaged storage, bad wiring on modified consoles.",
            Difficulty = "Medium",
            RiskLevel = "Medium.",
            ToolsNeeded = "Known-good HDD or no HDD test, USB storage, NAND reader/programmer for advanced cases, dashboard update files, console logs if available.",
            RecommendedSteps = "1. Remove the HDD and boot without it.\\n2. Clear system cache if accessible.\\n3. Try a known-good storage device.\\n4. Reapply the correct dashboard update.\\n5. If modified, verify NAND image, SMC, KV, and wiring.\\n6. Reflash a known-good NAND backup if needed.",
            Prevention = "Do not interrupt dashboard updates. Keep NAND backups for any console being serviced.",
            RelatedCodes = "E79, E71, E80, bad NAND, bad HDD",
            Notes = "Often storage or NAND related. Start with the easiest test: remove the HDD.",
        },
        new()
        {
            Issue = "RROD",
            Aliases = "red ring, red ring of death, 3 red lights, three red lights",
            Category = "General Hardware Failure",
            Meaning = "Three red lights indicate a general hardware failure. The secondary error code is needed for accurate diagnosis.",
            CommonBoards = "Mostly Xenon, Zephyr, Falcon, Opus; possible on all revisions",
            LikelyCauses = "GPU/CPU solder failure, overheating, power delivery issue, RAM fault, Southbridge fault, damaged motherboard.",
            Difficulty = "Advanced",
            RiskLevel = "High.",
            ToolsNeeded = "Secondary error code method, Torx tools, multimeter, thermal inspection, proper BGA/rework tools for board-level repair.",
            RecommendedSteps = "1. Get the secondary error code.\\n2. Check PSU light and power cable.\\n3. Remove HDD and accessories.\\n4. Inspect for dust, liquid damage, or board flex.\\n5. Match the secondary code to a repair entry.\\n6. Avoid clamp/heat-gun fixes.",
            Prevention = "Improve ventilation, clean dust, avoid heat cycles and board flex.",
            RelatedCodes = "0102, 0020, 0022, 0001, 0002, 0031",
            Notes = "RROD is a symptom, not a final diagnosis. Always use the secondary code.",
        },
        new()
        {
            Issue = "0102",
            Aliases = "0102, unknown hardware error, gpu solder, cold joint",
            Category = "GPU / BGA",
            Meaning = "Secondary error code commonly associated with GPU/BGA connection issues or general board-level hardware failure.",
            CommonBoards = "Xenon, Zephyr, Falcon",
            LikelyCauses = "GPU solder joint failure, board flex, overheating damage, failed previous reflow attempt.",
            Difficulty = "Advanced",
            RiskLevel = "High.",
            ToolsNeeded = "Secondary code confirmation, thermal inspection, microscope, proper BGA rework station.",
            RecommendedSteps = "1. Confirm secondary code.\\n2. Inspect board for warping or previous repair damage.\\n3. Check heatsink mount pressure and thermal paste.\\n4. Do not use towel/heat-gun fixes.\\n5. If repair is attempted, use proper BGA diagnostics and rework.",
            Prevention = "Keep console cool and avoid repeated overheating cycles.",
            RelatedCodes = "RROD, 0020, 0022",
            Notes = "One of the classic RROD-related codes.",
        },
        new()
        {
            Issue = "0022",
            Aliases = "0022, cpu error, gpu error, nand mismatch, bad flash",
            Category = "CPU / GPU / NAND",
            Meaning = "Can indicate CPU/GPU initialization failure or NAND/boot image mismatch depending on context.",
            CommonBoards = "All revisions",
            LikelyCauses = "Bad NAND flash, wrong image, CPU/GPU fault, damaged traces, failed update, bad wiring.",
            Difficulty = "Advanced",
            RiskLevel = "High.",
            ToolsNeeded = "NAND backup, NAND programmer, multimeter, board inspection tools, POST diagnostics if available.",
            RecommendedSteps = "1. Confirm secondary code.\\n2. If recently flashed, verify NAND image matches the console.\\n3. Restore known-good NAND backup.\\n4. Inspect wiring if modified.\\n5. Check for board damage around CPU/GPU and NAND points.",
            Prevention = "Always keep original NAND backups and verify image before flashing.",
            RelatedCodes = "0022, E79, bad NAND, no boot",
            Notes = "Context matters: after flashing, suspect NAND first.",
        },
        new()
        {
            Issue = "0031",
            Aliases = "0031, power fault, short, mosfet, power rail",
            Category = "Power Delivery",
            Meaning = "Power-related failure often associated with shorts, power rails, MOSFETs, or board-level electrical faults.",
            CommonBoards = "All revisions",
            LikelyCauses = "Shorted component, faulty MOSFET, damaged power rail, liquid damage, failed capacitor.",
            Difficulty = "Advanced",
            RiskLevel = "High.",
            ToolsNeeded = "Multimeter, bench PSU if experienced, boardview/reference, thermal camera or freeze spray, soldering tools.",
            RecommendedSteps = "1. Test with known-good PSU.\\n2. Inspect for liquid damage or burned components.\\n3. Check standby and main power rails.\\n4. Look for shorted capacitors or MOSFETs.\\n5. Do not repeatedly power cycle a shorted board.",
            Prevention = "Avoid liquid exposure and use correct power supply.",
            RelatedCodes = "0001, 0002, 0031, no power",
            Notes = "Electrical diagnostics required.",
        },
        new()
        {
            Issue = "No Video",
            Aliases = "no video, black screen, no display, hdmi no signal",
            Category = "Video Output",
            Meaning = "Console powers on but does not display video.",
            CommonBoards = "All revisions",
            LikelyCauses = "Wrong display mode, bad HDMI/AV cable, damaged port, ANA/HANA fault, GPU fault, failed update.",
            Difficulty = "Easy / Medium",
            RiskLevel = "Low to Medium.",
            ToolsNeeded = "Known-good HDMI/AV cable, known-good display, Torx tools for inspection if required.",
            RecommendedSteps = "1. Test another HDMI/AV cable.\\n2. Test another display.\\n3. Reset display settings.\\n4. Remove HDD and accessories.\\n5. Inspect HDMI/AV port.\\n6. If still no display, investigate video encoder/GPU path.",
            Prevention = "Avoid stressing HDMI/AV ports and keep console ventilated.",
            RelatedCodes = "E74, 1022, black screen",
            Notes = "Always rule out cable and display first.",
        },
        new()
        {
            Issue = "Disc Tray Stuck",
            Aliases = "tray stuck, disc tray, stuck tray, wont open, won't open",
            Category = "DVD Drive",
            Meaning = "DVD tray will not open or close correctly.",
            CommonBoards = "All revisions",
            LikelyCauses = "Worn drive belt, dirty mechanism, jammed tray, weak motor, misaligned gears.",
            Difficulty = "Easy / Medium",
            RiskLevel = "Low.",
            ToolsNeeded = "Replacement drive belt, isopropyl alcohol, cotton swabs, small tools.",
            RecommendedSteps = "1. Try manual eject hole.\\n2. Clean the tray belt and pulleys.\\n3. Replace worn belt.\\n4. Inspect tray gears and rails.\\n5. Do not force the tray.",
            Prevention = "Keep drive clean and avoid forcing discs/tray.",
            RelatedCodes = "Open Tray, unreadable disc",
            Notes = "Most stuck trays are belt related.",
        },
        new()
        {
            Issue = "Open Tray",
            Aliases = "open tray, unreadable disc, disc not reading, play dvd",
            Category = "DVD Drive",
            Meaning = "Console does not detect game discs correctly.",
            CommonBoards = "All revisions",
            LikelyCauses = "Dirty lens, weak laser, DVD key mismatch, worn spindle motor, damaged disc, wrong drive pairing.",
            Difficulty = "Medium",
            RiskLevel = "Medium.",
            ToolsNeeded = "Known-good disc, lens cleaning tools, replacement laser if needed, drive identification info.",
            RecommendedSteps = "1. Test multiple known-good discs.\\n2. Clean lens carefully.\\n3. Check if DVDs/CDs behave differently.\\n4. Inspect drive model and pairing context.\\n5. Replace laser only after ruling out simple causes.",
            Prevention = "Keep discs clean and avoid moving console while disc is spinning.",
            RelatedCodes = "Disc unreadable, Play DVD, DVD key",
            Notes = "If every game shows Play DVD, pairing/key mismatch may be involved.",
        },
        new()
        {
            Issue = "Overheating",
            Aliases = "overheat, 2 red lights, fan loud, thermal shutdown",
            Category = "Thermal",
            Meaning = "Console shuts down or warns due to excessive temperature.",
            CommonBoards = "All revisions, especially older Fat consoles",
            LikelyCauses = "Dust-clogged heatsink, bad thermal paste, failed fan, poor airflow, heatsink not seated.",
            Difficulty = "Easy / Medium",
            RiskLevel = "Medium.",
            ToolsNeeded = "Torx tools, compressed air, thermal paste, cleaning materials.",
            RecommendedSteps = "1. Clean vents and heatsinks.\\n2. Confirm fans spin correctly.\\n3. Replace thermal paste.\\n4. Check heatsink mounting.\\n5. Keep console in open airflow.",
            Prevention = "Regular cleaning and proper ventilation.",
            RelatedCodes = "2 red lights, thermal shutdown",
            Notes = "Do not run the console repeatedly while overheating.",
        },
        new()
        {
            Issue = "Bad NAND",
            Aliases = "bad nand, corrupted nand, bad flash, no boot after flash",
            Category = "NAND / Boot",
            Meaning = "NAND image corruption or wrong image causes boot failure.",
            CommonBoards = "All revisions",
            LikelyCauses = "Wrong NAND image, interrupted flash, bad blocks mishandled, incorrect build, damaged NAND wiring.",
            Difficulty = "Advanced",
            RiskLevel = "High.",
            ToolsNeeded = "Original NAND backup, NAND programmer, verification tools, soldering tools if needed.",
            RecommendedSteps = "1. Verify original NAND backup exists.\\n2. Compare CPU key/build data if applicable.\\n3. Check bad block remapping.\\n4. Rebuild image correctly.\\n5. Reflash and verify writes.\\n6. Inspect wiring if reads/writes are unstable.",
            Prevention = "Always create multiple verified NAND backups before writing.",
            RelatedCodes = "E79, E71, 0022, no boot",
            Notes = "Never overwrite the only NAND backup.",
        },
        new()
        {
            Issue = "Bad HDD",
            Aliases = "bad hdd, hard drive error, profile corrupt, storage issue",
            Category = "Storage",
            Meaning = "Hard drive or storage corruption causes boot, profile, update, or content issues.",
            CommonBoards = "All revisions",
            LikelyCauses = "Failing HDD, corrupted profile, bad sectors, damaged connector, interrupted update.",
            Difficulty = "Easy / Medium",
            RiskLevel = "Low.",
            ToolsNeeded = "Known-good HDD, USB storage, cache clear option if dashboard boots.",
            RecommendedSteps = "1. Remove HDD and boot console.\\n2. Test with known-good storage.\\n3. Clear cache if possible.\\n4. Redownload profile/content.\\n5. Replace HDD if failures continue.",
            Prevention = "Avoid disconnecting storage during updates or saves.",
            RelatedCodes = "E79, profile errors, content corruption",
            Notes = "Removing the HDD is one of the fastest diagnostics.",
        },
        new()
        {
            Issue = "No Power",
            Aliases = "no power, wont turn on, won't turn on, dead console",
            Category = "Power",
            Meaning = "Console does not power on.",
            CommonBoards = "All revisions",
            LikelyCauses = "Bad PSU, bad power button/RF board, shorted motherboard, damaged power connector.",
            Difficulty = "Easy / Advanced",
            RiskLevel = "Medium to High.",
            ToolsNeeded = "Known-good PSU, multimeter, replacement RF board if needed.",
            RecommendedSteps = "1. Check PSU light.\\n2. Try known-good power supply.\\n3. Remove accessories and storage.\\n4. Inspect power socket and RF board.\\n5. If PSU shuts off, investigate board short.",
            Prevention = "Use correct PSU and avoid liquid damage.",
            RelatedCodes = "0001, 0002, 0031",
            Notes = "PSU light behavior is important for diagnosis.",
        },
        new()
        {
            Issue = "Update Failed",
            Aliases = "update failed, system update stopped, update loop, dashboard update failed",
            Category = "Dashboard Update",
            Meaning = "Dashboard update cannot complete.",
            CommonBoards = "All revisions",
            LikelyCauses = "Wrong update files, bad storage, missing avatar data, corrupted NAND, modified system files.",
            Difficulty = "Medium",
            RiskLevel = "Medium.",
            ToolsNeeded = "Correct system update files, USB drive, known-good storage, NAND tools for advanced cases.",
            RecommendedSteps = "1. Confirm correct update version.\\n2. Use clean FAT32 USB.\\n3. Remove HDD and retry if storage corruption suspected.\\n4. Check update error/status code.\\n5. If modified, verify NAND/dashboard build.",
            Prevention = "Use correct official update files and do not interrupt updates.",
            RelatedCodes = "E71, E79, status codes, avatar update missing",
            Notes = "Avatar update issues can appear as incomplete dashboard installs.",
        },
    };

    public IReadOnlyList<RepairEntry> All => _entries;

    public RepairEntry? Find(string input)
    {
        string q = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Issue) == q ||
            x.Aliases.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Any(alias => Normalize(alias) == q));
    }

    public List<RepairEntry> Search(string query, int limit = 25)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
            return _entries.Take(limit).ToList();

        return _entries
            .Where(x =>
                x.Issue.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Aliases.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.CommonBoards.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.LikelyCauses.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.RelatedCodes.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToList();
    }

    public List<RepairEntry> ByCategory(string category, int limit = 25)
    {
        return _entries
            .Where(x => x.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
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
            .Replace("/", "")
            .ToUpperInvariant();
    }
}
