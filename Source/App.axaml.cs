using ASTRedux.Utils.Logging;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ManagedBass;

namespace ASTRedux;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.Exit += FinishProcess;
        }

        base.OnFrameworkInitializationCompleted();
    }
    private void FinishProcess(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        // clear all resources here
        Bass.Free();

        Logger.Message("BASS freed!", LogType.INFO);

        // finish by writing log
        if (Logger.VerbosityLevel > LogDetail.LOW)
            File.WriteAllText($"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log", Logger.LogOut.ToString());
    }
}