
namespace OrbitBackup.Services;

public class BackupOptions
{
    public string[]? SourceDirectories { get; set; }
    public string[]? SourceFiles { get; set; }
    public required string DestinationDirectory { get; set; }
    public required string DestinationBackupFormat { get; set; }
    public required bool EnableCompression { get; set; }
    public required int MaxBackupCount { get; set; }
}