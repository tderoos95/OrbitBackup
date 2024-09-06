using Microsoft.Extensions.Options;
using OrbitBackup.Services;
using OrbitBackup.Strategies;

namespace OrbitBackup.Factories;

public static class StrategyFactory
{
    public static IBackupStrategy Create(IOptions<BackupOptions> options)
    {
        var backupTime = DateTime.Now;

        IBackupStrategy strategy = new LocalBackupStrategy(options)
        {
            BackupTime = backupTime
        };

        if (options.Value.EnableCompression)
        {
            strategy = new ZipBackupStrategy(options, strategy)
            {
                BackupTime = backupTime
            };
        }

        return strategy;
    }
}