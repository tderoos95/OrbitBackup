using Microsoft.Extensions.Options;
using OrbitBackup.Strategies;
using OrbitBackup.Factories;
using FluentValidation;
using Serilog;

namespace OrbitBackup.Services;

public class BackupService(IOptions<BackupOptions> settings, IValidator<BackupOptions> validator) : IBackupService
{
    private readonly IBackupStrategy strategy = StrategyFactory.Create(settings);

    public void ValidateOptions()
    {
        Log.Information("Validating options...");
        var validationResult = validator.Validate(settings.Value);
        
        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(error => Log.Error(error.ErrorMessage));
            throw new ValidationException(validationResult.Errors);
        }

        Log.Information("Options are valid.");
    }

    private void EnsureDestinationExists()
    {
        Directory.CreateDirectory(settings.Value.DestinationDirectory);
        Log.Information("Destination directory: {DestinationDirectory}", settings.Value.DestinationDirectory);
    }

    public void Backup()
    {
        EnsureDestinationExists();

        Log.Information("Max backup count: {MaxAmountOfBackups}", settings.Value.MaxBackupCount);
        Log.Information("Compression enabled: {EnableCompression}", settings.Value.EnableCompression);
        Log.Information("Starting backup...");

        strategy.BeforeBackup();
        strategy.Backup();
        strategy.AfterBackup();
        strategy.RemoveExceedingBackups();

        Log.Information("Backup completed successfully.");
    }
}