using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fill_ADSK_Parameters
{
    public static class PositionRuleCatalog
    {
        private const string CsvFileName = "position_rules.csv";

        private static readonly object CacheLock =
        new object();

        private static IReadOnlyList<PositionRule> cachedRules =
        new List<PositionRule>();

        private static DateTime cachedWriteTimeUtc =
        DateTime.MinValue;

        public static bool TryGetBasePosition(string name, out string basePosition)
        {
            basePosition = "";

            if (string.IsNullOrEmpty(name))
                return false;

            string normalizedName =
            NormalizeText(name);

            foreach (PositionRule rule in GetRules().OrderByDescending(x => x.Pattern.Length))
            {
                string normalizedRule =
                NormalizeText(rule.Pattern);

                if (normalizedName.IndexOf(
                normalizedRule,
                StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    basePosition =
                    rule.BasePosition.ToString();

                    return true;
                }
            }

            return false;
        }

        public static int RuleCount
        {
            get { return GetRules().Count; }
        }

        public static string SourcePath
        {
            get { return GetRulesCsvPath(); }
        }

        private static IReadOnlyList<PositionRule> GetRules()
        {
            string csvPath =
            GetRulesCsvPath();

            DateTime writeTimeUtc =
            File.Exists(csvPath)
                ? File.GetLastWriteTimeUtc(csvPath)
                : DateTime.MinValue;

            lock (CacheLock)
            {
                if (writeTimeUtc == cachedWriteTimeUtc)
                    return cachedRules;

                cachedRules =
                LoadRules(csvPath);

                cachedWriteTimeUtc =
                writeTimeUtc;

                return cachedRules;
            }
        }

        private static IReadOnlyList<PositionRule> LoadRules(string csvPath)
        {
            if (!File.Exists(csvPath))
                return new List<PositionRule>();

            List<PositionRule> rules =
            new List<PositionRule>();

            foreach (string line in File.ReadLines(csvPath, Encoding.UTF8))
            {
                string trimmed =
                line.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                string[] columns =
                ParseCsvLine(line);

                if (columns.Length < 3)
                    continue;

                if (columns[0].Equals("group", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!int.TryParse(columns[2], out int basePosition))
                    continue;

                if (string.IsNullOrWhiteSpace(columns[1]))
                    continue;

                rules.Add(new PositionRule(
                    columns[0].Trim(),
                    columns[1].Trim(),
                    basePosition));
            }

            return rules;
        }

        private static string GetRulesCsvPath()
        {
            string assemblyPath =
            Assembly.GetExecutingAssembly().Location;

            string assemblyDirectory =
            Path.GetDirectoryName(assemblyPath);

            return Path.Combine(assemblyDirectory, CsvFileName);
        }

        private static string[] ParseCsvLine(string line)
        {
            List<string> columns =
            new List<string>();

            StringBuilder current =
            new StringBuilder();

            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c =
                line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                if (c == ';' && !inQuotes)
                {
                    columns.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            columns.Add(current.ToString());

            return columns.ToArray();
        }

        private static string NormalizeText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            string normalized =
            value.Replace('ё', 'е')
            .Replace('Ё', 'Е')
            .ToLowerInvariant();

            while (normalized.Contains("  "))
                normalized = normalized.Replace("  ", " ");

            return normalized.Trim();
        }
    }
}
