using Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace Library.Services
{
    /// <summary>
    /// Watches for new drives being plugged in. When a drive is detected, it checks if the drive has a valid
    /// Config.csv (OpenWhenPlugged=TRUE), reads the Games.csv file and raises the DriveReady
    /// event with the found games.
    /// </summary>
    public class DriveWatcherService : IDisposable
    {
        private ManagementEventWatcher? _watcher;

        public event Action<string, List<Game>>? DriveReady;

        public void Start()
        {
            if (_watcher != null) return; // mar fut

            var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += OnVolumeArrived;
            _watcher.Start();
        }

        public void Stop()
        {
            if (_watcher == null) return;

            _watcher.EventArrived -= OnVolumeArrived;
            _watcher.Stop();
            _watcher.Dispose();
            _watcher = null;
        }

        /// <summary>
        /// It goes through all currently connected and ready drives,
        /// and for the first one that has a valid Config.csv file, it loads the games
        /// just as if the drive had just been plugged in.
        /// </summary>
        public void ScanConnectedDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;

                if (TryLoadDrive(drive.RootDirectory.FullName))
                {
                    break; // az elso megfelelonel megallunk
                }
            }
        }

        private void OnVolumeArrived(object sender, EventArrivedEventArgs e)
        {
            string? driveLetter = e.NewEvent["DriveName"]?.ToString(); // pl. "E:"
            if (string.IsNullOrEmpty(driveLetter)) return;

            TryLoadDrive(driveLetter + "\\");
        }

        private bool TryLoadDrive(string drivePath)
        {
            var config = ReadConfig(drivePath);
            if (!config.TryGetValue("OpenWhenPlugged", out string? value) || value != "TRUE")
            {
                return false;
            }

            List<Game> games = GetGames(drivePath);
            foreach (var game in games)
            {
                GameScanner.DiscoverExecutables(game, drivePath);
            }

            DriveReady?.Invoke(drivePath, games);
            return true;
        }

        private Dictionary<string, string> ReadConfig(string drivePath)
        {
            Dictionary<string, string> config = new();
            string configFilePath = Path.Combine(drivePath, "Config.csv");

            if (File.Exists(configFilePath))
            {
                using StreamReader sr = new StreamReader(configFilePath);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        config[parts[0]] = parts[1];
                    }
                }
            }
            return config;
        }

        private List<Game> GetGames(string drivePath)
        {
            string gamesFilePath = Path.Combine(drivePath, "Games.csv");
            List<Game> games = new();

            if (!File.Exists(gamesFilePath)) return games;

            using StreamReader sr = new StreamReader(gamesFilePath);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 2 && int.TryParse(parts[0], out int id))
                {
                    games.Add(new Game(id, parts[1]));
                }
            }
            return games;
        }

        public void Dispose() => Stop();
    }
}
