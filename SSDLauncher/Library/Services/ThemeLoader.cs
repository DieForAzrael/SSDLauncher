using Library.Models;
using System;
using System.IO;
using System.Text.Json;

namespace Library.Services
{
    public static class ThemeLoader
    {
        /// <summary>
        /// Reads the {drivePath}\Design\theme.json file. If it doesn't exist,
        /// or if the content is invalid, it returns a default LauncherTheme.
        /// </summary>
        public static LauncherTheme LoadTheme(string drivePath)
        {
            string themePath = Path.Combine(drivePath, "Design", "theme.json");

            if (!File.Exists(themePath))
            {
                return new LauncherTheme();
            }

            try
            {
                string json = File.ReadAllText(themePath);
                var theme = JsonSerializer.Deserialize<LauncherTheme>(json);
                return theme ?? new LauncherTheme();
            }
            catch (Exception)
            {
                // default theme if the file is invalid or cannot be read
                return new LauncherTheme();
            }
        }

        /// <summary>
        /// Finds the background image file in the {drivePath}\Design folder among the supported
        /// extensions. Returns null if no such file is found.
        /// </summary>
        public static string? FindBackgroundImage(string drivePath)
        {
            string[] extensions = { ".jpg", ".png", ".jpeg", ".webp" };
            string designFolder = Path.Combine(drivePath, "Design");

            foreach (var ext in extensions)
            {
                string fullPath = Path.Combine(designFolder, $"background{ext}");
                if (File.Exists(fullPath)) return fullPath;
            }
            return null;
        }
    }
}
