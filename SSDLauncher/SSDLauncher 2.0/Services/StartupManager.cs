using Microsoft.Win32;

namespace Library.Services
{
    public static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "SSDLauncher";

        public static bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            return key?.GetValue(AppName) != null;
        }

        public static void Enable(string exePath)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                key?.SetValue(AppName, $"\"{exePath}\"");
            }
            catch
            {
                // Ignore exceptions, as this is a non-critical operation and we don't want to crash the application if it fails.
            }
        }

        public static void Disable()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                key?.DeleteValue(AppName, throwOnMissingValue: false);
            }
            catch
            {
            }
        }
    }
}
