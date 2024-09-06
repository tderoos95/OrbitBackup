using Microsoft.Extensions.Options;
using OrbitBackup.Services;

namespace OrbitBackup.Strategies;

public class ZipBackupStrategy(IOptions<BackupOptions> options, IBackupStrategy innerStrategy) : IBackupStrategy
{
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
        
        // Todo: Zip the backup
    }
}