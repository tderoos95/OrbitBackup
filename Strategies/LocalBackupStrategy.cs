using Microsoft.Extensions.Options;
using OrbitBackup.Services;
using Serilog;

namespace OrbitBackup.Strategies;

public class LocalBackupStrategy(IOptions<BackupOptions> options) : IBackupStrategy
{
    private DirectoryInfo? destinationDirectory;

    public DateTime BackupTime { get; set; }

    public void BeforeBackup()
    {
        EnsureDestinationExists();
    }

    private void EnsureDestinationExists()
    {
        string backupDestinationName = $"{BackupTime.ToString(options.Value.DestinationBackupFormat)}";
        string backupDestinationPath = Path.Combine(options.Value.DestinationDirectory, backupDestinationName);
        destinationDirectory = Directory.CreateDirectory(backupDestinationPath);
        Log.Information("Destination directory for local backup: {DestinationDirectory}", destinationDirectory.FullName);
    }

    public void Backup()
    {
        BackupSourceDirectories();
        BackupSourceFiles();
    }

    private void BackupSourceDirectories()
    {
        ArgumentNullException.ThrowIfNull(destinationDirectory);

        if (options.Value.SourceDirectories == null)
        {
            return;
        }

        Log.Information("Backing up {NumberOfSourceDirectories} source directorie(s)...", options.Value.SourceDirectories.Length);

        foreach (string sourceDirectory in options.Value.SourceDirectories)
        {
            string sourceDirectoryName = new DirectoryInfo(sourceDirectory).Name;
            string destinationDirectoryPath = Path.Combine(destinationDirectory.FullName, sourceDirectoryName);
            Directory.CreateDirectory(destinationDirectoryPath);
            Log.Information("{SourceDirectory} -> {DestinationDirectory}", sourceDirectory, destinationDirectoryPath);

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destinationDirectoryPath, fileName);
                File.Copy(file, destinationFilePath);
            }
        }
    }

    private void BackupSourceFiles()
    {
        ArgumentNullException.ThrowIfNull(destinationDirectory);
        
        if (options.Value.SourceFiles == null)
        {
            return;
        }

        Log.Information("Backing up {NumberOfSourceFiles} source file(s)...", options.Value.SourceFiles.Length);

        foreach (string sourceFile in options.Value.SourceFiles)
        {
            string fileName = Path.GetFileName(sourceFile);
            string destinationFilePath = Path.Combine(destinationDirectory.FullName, fileName);
            File.Copy(sourceFile, destinationFilePath);
            Log.Information("{SourceFile} -> {DestinationFile}", sourceFile, destinationFilePath);
        }
    }

    public void AfterBackup()
    { }

    public void RemoveExceedingBackups()
    {
        ArgumentNullException.ThrowIfNull(destinationDirectory?.Parent);

        var directories = destinationDirectory.Parent.GetDirectories();
        if (directories.Length <= options.Value.MaxBackupCount)
        {
            return;
        }

        var directoriesToDelete = directories
            .OrderByDescending(d => d.CreationTime)
            .Skip(options.Value.MaxBackupCount);

        Log.Information("Amount of backup directories: {AmountOfBackups}", directories.Length);
        Log.Information("Max backup count: {MaxAmountOfBackups}", options.Value.MaxBackupCount);
        Log.Information("Removing {NumberOfBackupsToRemove} exceeding backup directories...", directoriesToDelete.Count());

        foreach (var directory in directories)
        {
            directory.Delete();
        }
    }
}