using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library.Models;
using Library.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using SSDLauncher_2._0;

namespace SSDLauncher_2._0.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DriveWatcherService _driveWatcher;

        [ObservableProperty]
        private ObservableCollection<Game> games = new();

        [ObservableProperty]
        private string currentDrivePath = string.Empty;

        public MainViewModel()
        {
            _driveWatcher = new DriveWatcherService();
            _driveWatcher.DriveReady += HandleDriveReady;
            _driveWatcher.Start();
        }

        private void HandleDriveReady(string drivePath, List<Game> newGames)
        {
            var theme = ThemeLoader.LoadTheme(drivePath);
            string? backgroundPath = ThemeLoader.FindBackgroundImage(drivePath);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentDrivePath = drivePath;
                Games.Clear();
                foreach (var game in newGames)
                {
                    game.GetCoverImagePath(drivePath);
                    Games.Add(game);
                }

                ((App)System.Windows.Application.Current).ApplyTheme(theme, backgroundPath);
            });
        }
        public void ScanNow()
        {
            _driveWatcher.ScanConnectedDrives();
        }

        [RelayCommand]
        private void PlayGame(Game game)
        {
            GameLauncher.LaunchGame(game, CurrentDrivePath);
        }
        [RelayCommand]
        private void OpenGameSettings(Game game)
        {
            var window = new GameSettingsWindow(game)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            window.ShowDialog();
        }
    }
}