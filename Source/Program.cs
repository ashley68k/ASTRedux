using ASTRedux.Utils;
using ASTRedux.Utils.Consts;
using ASTRedux.Utils.Helpers;
using ASTRedux.Utils.Logging;
using ManagedBass;
using System.CommandLine;
using System.Reflection.Metadata.Ecma335;
using Avalonia;

namespace ASTRedux;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
