using Cocona;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrbitBackup.Services;
using OrbitBackup.Validators;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = CoconaApp.CreateBuilder();

builder.Services.Configure<BackupOptions>(builder.Configuration.GetSection("Backup"));
builder.Services.AddScoped<IValidator<BackupOptions>, BackupOptionsValidator>();
builder.Services.AddTransient<IBackupService, BackupService>();

var app = builder.Build();

app.Run(() => {
    var service = app.Services.GetRequiredService<IBackupService>();
    service.ValidateOptions();
    service.Backup();
});