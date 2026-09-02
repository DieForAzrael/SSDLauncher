using Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Library.Services
{
    public static class GameLauncher
    {
        /// <summary>
        /// Starts the specified game by launching its executable file.
        /// </summary>
        public static bool LaunchGame(Game game, string drivePath)
        {
            string? fullExePath = game.GetActiveExecutable(drivePath);

            if (string.IsNullOrEmpty(fullExePath) || !File.Exists(fullExePath))
            {
                Console.WriteLine($"[HIBA] A futtatható fájl nem található: {fullExePath}");
                return false;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fullExePath,
                    WorkingDirectory = Path.GetDirectoryName(fullExePath),
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HIBA] Nem sikerült elindítani a játékot: {ex.Message}");
                return false;
            }
        }
    }
}
