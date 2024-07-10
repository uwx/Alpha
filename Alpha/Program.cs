﻿using System.Reflection;
using Alpha.Gui.Windows;
using Alpha.Gui.Windows.Ftue;
using Alpha.Services;
using Alpha.Services.Excel;
using Alpha.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Alpha;

public class Program {
    public static string AppDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Alpha"
    );
    public static IHost Host = null!;

    public static void Main() {
        var overrideAppDir = Environment.GetEnvironmentVariable("ALPHA_APPDIR");
        if (overrideAppDir is not null) AppDir = overrideAppDir;

        if (!Directory.Exists(AppDir)) Directory.CreateDirectory(AppDir);
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(AppDir, "Alpha.log"))
            .MinimumLevel.Debug()
            .CreateLogger();

        var builder = new HostApplicationBuilder();
        builder.Environment.ContentRootPath = AppDir;
        builder.Services.AddSerilog();

        builder.Services.AddSingleton(Config.Load());

        builder.Services.AddSingletonHostedService<GuiService>();
        builder.Services.AddSingletonHostedService<WindowManagerService>();
        builder.Services.AddSingleton<GameDataService>();
        builder.Services.AddSingleton<PathService>();
        builder.Services.AddSingleton<ExcelService>();

        builder.Services.AddScoped<FtueWindow>();
        builder.Services.AddScoped<ExcelWindow>();
        builder.Services.AddScoped<SettingsWindow>();
        builder.Services.AddScoped<FilesystemWindow>();

        Log.Information("Alpha is starting, please wait... {Version}", Assembly.GetExecutingAssembly().GetName().Version);
        Host = builder.Build();

        // Could use a hosted service here but I am lazy
        Host.Services.GetRequiredService<GameDataService>();
        Host.Services.GetRequiredService<PathService>();

        Host.Start();
        Host.WaitForShutdown();
    }
}
