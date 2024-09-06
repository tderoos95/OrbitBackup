using System.IO.Compression;
using Microsoft.Extensions.Options;
using OrbitBackup.Services;
using Serilog;

namespace OrbitBackup.Strategies;

public class ZipBackupStrategy(IOptions<BackupOptions> options, IBackupStrategy innerStrategy) : IBackupStrategy
{
    private DirectoryInfo? destinationDirectory;

    public DateTime BackupTime { get; set; }

    public void BeforeBackup()
    {
        innerStrategy.BeforeBackup();
    }

    public void Backup()
    {
        innerStrategy.Backup();
    }

    public void AfterBackup()
    {
        innerStrategy.AfterBackup();
    
        EnsureDestinationExists();
        ZipBackup();
    }

    private void EnsureDestinationExists()
    {
        string backupDestinationName = $"{BackupTime.ToString(options.Value.DestinationBackupFormat)}";
        string backupDestinationPath = Path.Combine(options.Value.DestinationDirectory, backupDestinationName);
        destinationDirectory = new DirectoryInfo(backupDestinationPath);

        if (!destinationDirectory.Exists)
        {
            throw new DirectoryNotFoundException($"The destination directory {destinationDirectory.FullName} does not exist.");
        }
    }

    private void ZipBackup()
    {
        ArgumentNullException.ThrowIfNull(destinationDirectory?.Parent);

        // Create the zip file
        string zipFileName = $"{destinationDirectory.Name}.zip";
        string zipFilePath = Path.Combine(destinationDirectory.Parent.FullName, zipFileName);

        Log.Information("{DestinationDirectory} -> {ZipFilePath}", destinationDirectory.FullName, zipFilePath);
        ZipFile.CreateFromDirectory(destinationDirectory.FullName, zipFilePath);

        // Delete the original backup
        Log.Information("Deleting directory {DestinationDirectory}...", destinationDirectory.FullName);
        destinationDirectory.Delete(true);
    }

    public void RemoveExceedingBackups()
    {
        // Do not call innerStrategy because we're using zip files instead of directories

        var backupDirectory = new DirectoryInfo(options.Value.DestinationDirectory);
        if (!backupDirectory.Exists)
        {
            throw new DirectoryNotFoundException($"The backup directory {backupDirectory.FullName} does not exist.");
        }

        var backups = backupDirectory.GetFiles("*.zip");
        var backupsToRemove = backups
            .OrderByDescending(f => f.CreationTime)
            .Skip(options.Value.MaxBackupCount);

        Log.Information("Amount of backups: {AmountOfBackups}", backups.Length);
        Log.Information("Max backup count: {MaxAmountOfBackups}", options.Value.MaxBackupCount);
        Log.Information("Removing {NumberOfBackupsToRemove} exceeding zip backup(s)...", backupsToRemove.Count());

        foreach (var backup in backupsToRemove)
        {
            backup.Delete();
            Log.Information("Deleted {Backup}", backup.FullName);
        }
    }
}