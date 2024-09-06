using Microsoft.Extensions.Options;
using OrbitBackup.Services;

namespace OrbitBackup.Strategies;

public class LocalBackupStrategy(IOptions<BackupOptions> options) : IBackupStrategy
{
    private DirectoryInfo? destinationDirectory;

    public void BeforeBackup()
    {
        EnsureDestinationExists();
    }

    private void EnsureDestinationExists()
    {
        string backupDestinationName = $"{DateTime.Now.ToString(options.Value.DestinationBackupFormat)}";
        string backupDestinationPath = Path.Combine(options.Value.DestinationDirectory, backupDestinationName);
        destinationDirectory = Directory.CreateDirectory(backupDestinationPath);
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

        foreach (string sourceDirectory in options.Value.SourceDirectories)
        {
            string sourceDirectoryName = new DirectoryInfo(sourceDirectory).Name;
            string destinationDirectoryPath = Path.Combine(destinationDirectory.FullName, sourceDirectoryName);
            Directory.CreateDirectory(destinationDirectoryPath);

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

        foreach (string sourceFile in options.Value.SourceFiles)
        {
            string fileName = Path.GetFileName(sourceFile);
            string destinationFilePath = Path.Combine(destinationDirectory.FullName, fileName);
            File.Copy(sourceFile, destinationFilePath);
        }
    }

    public void AfterBackup()
    { }
}