namespace SSDLauncher_2._0.ViewModels
{
    /// <summary>
    /// Simple class to represent an executable entry with a relative path and an active status.
    /// </summary>
    public class ExecutableEntry
    {
        public string RelativePath { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
