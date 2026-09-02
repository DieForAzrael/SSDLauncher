using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Services
{
    public class GameScanner
    {
        private static readonly List<string> DefaultIgnoredKeywords = new()
        {
            "unins", "crash", "setup", "redist", "unitycrash", "config", "launcher"
        };
        private static List<string> _ignoredKeywords = new();
        /// <summary>
        /// Add a keyword or exe name to the blacklist and updates the in-memory filter.
        /// </summary>
        public static bool AddToBlacklist(string keywordOrExeName)
        {
            if (string.IsNullOrWhiteSpace(keywordOrExeName)) return false;

            string cleanKeyword = keywordOrExeName.Trim().ToLower();

            if (_ignoredKeywords.Contains(cleanKeyword)) return false;

            _ignoredKeywords.Add(cleanKeyword);

            string blacklistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Blacklist.txt");

            try
            {
                File.AppendAllLines(blacklistPath, new[] { cleanKeyword });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a feketelista mentésekor: {ex.Message}");
                return false;
            }
        }

        static GameScanner()
        {
            LoadBlacklist();
        }

        public static void LoadBlacklist()
        {
            string blacklistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Blacklist.txt");

            if (File.Exists(blacklistPath))
            {

                _ignoredKeywords = File.ReadAllLines(blacklistPath)
                    .Select(line => line.Trim().ToLower())
                    .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    .ToList();
            }
            else
            {
                _ignoredKeywords = new List<string>(DefaultIgnoredKeywords);
            }
        }
        /// <summary>
        /// Discovers executable files in the game folder (Games\{Id}) and populates the Game object's Executables dictionary.
        /// </summary>
        public static void DiscoverExecutables(Game game, string drivePath)
        {
            string gameFolderPath = Path.Combine(drivePath, "Games", game.Id.ToString());
            if(!Directory.Exists(gameFolderPath))
            {
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(gameFolderPath);
            var exeFiles = dir.GetFiles("*.exe", SearchOption.AllDirectories)
                .Where(f => !IsIgnoredExecutable(f.Name))
                .OrderByDescending(f => f.Length)
                .ToList();
            if (exeFiles.Count == 0) return;
            foreach (var exeFile in exeFiles)
            {
                string relativePath = Path.GetRelativePath(gameFolderPath, exeFile.FullName);

                if (!game.Executables.ContainsKey(relativePath))
                {
                    game.Executables.Add(relativePath, false);
                }
            }
            if (!game.Executables.ContainsValue(true) && game.Executables.Count > 0)
            {
                string firstKey = game.Executables.Keys.First();
                game.Executables[firstKey] = true;
            }
        }
        private static bool IsIgnoredExecutable(string fileName)
        {
            string lowerName = fileName.ToLower();
            return _ignoredKeywords.Any(keyword => lowerName.Contains(keyword));
        }
    }
}
