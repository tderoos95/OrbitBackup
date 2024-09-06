using FluentValidation;
using OrbitBackup.Services;

namespace OrbitBackup.Validators;

public class BackupOptionsValidator : AbstractValidator<BackupOptions>
{
    public BackupOptionsValidator()
    {
        RuleForEach(x => x.SourceDirectories)
            .Must((options, directory) => Directory.Exists(directory))
            .WithMessage((options, directory) => $"The source directory '{directory}' does not exist.");

        RuleForEach(x => x.SourceFiles)
            .Must((options, file) => File.Exists(file))
            .WithMessage((options, file) => $"The source file '{file}' does not exist.");

        RuleFor(x => x.DestinationBackupFormat)
            .NotEmpty()
            .WithMessage("Destination backup format must not be empty.");

        RuleFor(x => x.MaxBackupCount)
            .GreaterThan(0)
            .WithMessage("Max backup count must be greater than 0.");
    }
}
