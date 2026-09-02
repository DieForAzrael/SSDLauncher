using Hardcodet.Wpf.TaskbarNotification;
using Library.Models;
using Library.Services;
using SSDLauncher_2._0.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SSDLauncher_2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? _notifyIcon;
        public bool IsExiting { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();

            string? exePath = Environment.ProcessPath;
            if (exePath != null)
            {
                StartupManager.Enable(exePath);
            }

            SetupTrayIcon();
        }

        private void SetupTrayIcon()
        {
            _notifyIcon = new TaskbarIcon
            {
                IconSource = BitmapFrame.Create(new Uri("pack://application:,,,/icon.ico")),
                ToolTipText = "SSD Launcher"
            };

            var openItem = new MenuItem { Header = "Open" };
            openItem.Click += (s, e) => ShowMainWindow();

            var scanItem = new MenuItem { Header = "Scan" };
            scanItem.Click += (s, e) => TriggerScan();

            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, e) => ExitApplication();

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(scanItem);
            contextMenu.Items.Add(exitItem);
            _notifyIcon.ContextMenu = contextMenu;

            _notifyIcon.TrayMouseDoubleClick += (s, e) => ShowMainWindow();
        }

        public void ShowMainWindow()
        {
            if (MainWindow == null) return;
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Topmost = true;
            MainWindow.Topmost = false;
            MainWindow.Activate();
        }

        private void TriggerScan()
        {
            if (MainWindow?.DataContext is MainViewModel vm)
            {
                vm.ScanNow();
            }
        }

        private void ExitApplication()
        {
            IsExiting = true;
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }

        public void ApplyTheme(LauncherTheme theme, string? backgroundImagePath)
        {
            Resources["AccentBrush"] = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(theme.AccentColor));

            Resources["AppBackgroundBrush"] = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(theme.BackgroundColor));

            Resources["AppFontFamily"] = new FontFamily(theme.FontFamily);

            if (backgroundImagePath != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(backgroundImagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                Resources["AppBackgroundImageBrush"] = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
            }
            else
            {
                Resources["AppBackgroundImageBrush"] = new ImageBrush();
            }
        }
    }
}

