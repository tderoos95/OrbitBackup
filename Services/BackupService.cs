using Microsoft.Extensions.Options;
using OrbitBackup.Strategies;
using OrbitBackup.Factories;

namespace OrbitBackup.Services;

public class BackupService(IOptions<BackupOptions> settings) : IBackupService
{
    private readonly IBackupStrategy strategy = StrategyFactory.Create(settings);

    private void EnsureDestinationExists()
    {
        Directory.CreateDirectory(settings.Value.DestinationDirectory);
    }

    public void Backup()
    {
        EnsureDestinationExists();

        strategy.BeforeBackup();
        strategy.Backup();
        strategy.AfterBackup();
        strategy.RemoveExceedingBackups();
    }
}