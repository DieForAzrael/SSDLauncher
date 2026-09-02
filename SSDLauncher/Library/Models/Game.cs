using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, bool> Executables { get; set; } = new();
        public string? CoverImagePath { get; set; }

        public Game(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public void GetCoverImagePath(string drivePath)
        {
            string[] extensions = { ".jpg", ".png", ".webp", ".jpeg" };

            foreach (var ext in extensions)
            {
                string fullPath = Path.Combine(drivePath, "Images", $"{Id}{ext}");
                if (File.Exists(fullPath))
                {
                    CoverImagePath = fullPath;
                    return;
                }
            }
            CoverImagePath = null;
        }
        public string? GetActiveExecutable(string drivePath)
        {
            var activePair = Executables.FirstOrDefault(x => x.Value == true);

            string activeExeName = activePair.Key ?? Executables.Keys.FirstOrDefault();

            if (string.IsNullOrEmpty(activeExeName)) return null;

            return Path.Combine(drivePath, "Games", this.Id.ToString(), activeExeName);
        }

        /// <summary>
        /// Sets the active executable for the game. Returns true if successful, false if the specified executable does not exist.
        /// </summary>
        public bool SetActiveExecutable(string relativeExePath)
        {
            if (!Executables.ContainsKey(relativeExePath)) return false;

            foreach (var key in Executables.Keys.ToList())
            {
                Executables[key] = false;
            }
            Executables[relativeExePath] = true;
            return true;
        }
        public void RemoveExecutable(string relativeExePath)
        {
            if (!Executables.Remove(relativeExePath)) return;

            if (!Executables.ContainsValue(true) && Executables.Count > 0)
            {
                string firstKey = Executables.Keys.First();
                Executables[firstKey] = true;
            }
        }
    }
}
