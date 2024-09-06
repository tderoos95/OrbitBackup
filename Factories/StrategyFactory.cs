using Microsoft.Extensions.Options;
using OrbitBackup.Services;
using OrbitBackup.Strategies;

namespace OrbitBackup.Factories;

public static class StrategyFactory
{
    public static IBackupStrategy Create(IOptions<BackupOptions> options)
    {
        IBackupStrategy strategy = new LocalBackupStrategy(options);

        if (options.Value.EnableCompression)
        {
            strategy = new ZipBackupStrategy(options, strategy);
        }

        return strategy;
    }
}