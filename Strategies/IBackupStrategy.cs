namespace OrbitBackup.Strategies;

public interface IBackupStrategy
{
    void BeforeBackup();
    void Backup();
    void AfterBackup();
}