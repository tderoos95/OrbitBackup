using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrbitBackup.Services;

var builder = CoconaApp.CreateBuilder();

builder.Services.Configure<BackupOptions>(builder.Configuration.GetSection("Backup"));
builder.Services.AddTransient<IBackupService, BackupService>();

var app = builder.Build();

app.Run(() => {
    var service = app.Services.GetRequiredService<IBackupService>();
    service.Backup();
});