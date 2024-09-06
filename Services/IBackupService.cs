namespace OrbitBackup.Services;

public interface IBackupService
{
    void ValidateOptions();
    void Backup();
}