using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library.Models;
using Library.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SSDLauncher_2._0.ViewModels
{
    public partial class GameSettingsViewModel : ObservableObject
    {
        private readonly Game _game;

        [ObservableProperty]
        private ObservableCollection<ExecutableEntry> executables = new();

        public GameSettingsViewModel(Game game)
        {
            _game = game;
            LoadExecutables();
        }

        private void LoadExecutables()
        {
            Executables = new ObservableCollection<ExecutableEntry>(
                _game.Executables.Select(kvp => new ExecutableEntry
                {
                    RelativePath = kvp.Key,
                    IsActive = kvp.Value
                }));
        }

        [RelayCommand]
        private void SelectExecutable(ExecutableEntry entry)
        {
            _game.SetActiveExecutable(entry.RelativePath);
            LoadExecutables();
        }

        [RelayCommand]
        private void BlacklistExecutable(ExecutableEntry entry)
        {
            string fileName = Path.GetFileName(entry.RelativePath);
            GameScanner.AddToBlacklist(fileName);
            _game.RemoveExecutable(entry.RelativePath);
            LoadExecutables();
        }
    }
}
