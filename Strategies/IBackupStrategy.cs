namespace OrbitBackup.Strategies;

public interface IBackupStrategy
{
    DateTime BackupTime { get; set; }

    void BeforeBackup();
    void Backup();
    void AfterBackup();
    void RemoveExceedingBackups();
}