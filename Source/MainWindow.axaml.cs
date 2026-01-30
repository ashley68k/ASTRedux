using ASTRedux.Utils;
using ASTRedux.Utils.Logging;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ManagedBass;

namespace ASTRedux;
public partial class MainWindow : Window
{
    private const int SFX_MODE = 0;
    private const int MUSIC_MODE = 1;

    private FilePickerFileType soundType = new("Dead Rising Sound Files")
    {
        Patterns = new[] { "*.rSoundSnd", "*.snd" },
        AppleUniformTypeIdentifiers = null,
        MimeTypes = null
    };

    private FilePickerFileType musicType = new("Dead Rising Music Files")
    {
        Patterns = new[] { "*.rSoundAst", "*.ast" },
        AppleUniformTypeIdentifiers = null,
        MimeTypes = null
    };

    public MainWindow()
    {
        InitializeComponent();

        Logger.OnLogMessage += WriteLog;
        Logger.VerbosityLevel = LogDetail.LOW;

        SetLabels();

        Logger.Message("Welcome to ASTRedux!");
    }

    private void SetLabels()
    {
        // this gets called at startup, just don't allow this to run
        // if we're gonna end up accessing a null object
        if (ModeTab == null)
            return;

        switch (ModeTab.SelectedIndex)
        {
            case SFX_MODE:
                DecodeBtn.Content = "Decode rSound to Folder";
                EncodeBtn.Content = "Encode Folder to rSound";
                break;
            case MUSIC_MODE:
                DecodeBtn.Content = "Decode AST to Audio";
                EncodeBtn.Content = "Encode Audio to AST";
                break;
        }
    }

    // convert Dead Rising format file to standard audio format
    private async void Decode(object? sender, RoutedEventArgs e)
    {
        if (ModeTab == null)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        switch (ModeTab.SelectedIndex)
        {
            case SFX_MODE:
                // file -> folder
                var soundIn = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select rSoundSnd file to decode from",
                    FileTypeFilter = new[] { soundType },
                    AllowMultiple = false
                });
                var soundOut = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select folder to decode to",
                    AllowMultiple = false
                });
                if (soundIn.Any() && soundOut.Any())
                    ConversionPipeline.DecodeSound(soundIn[0], soundOut[0]);
                else
                    Logger.Message("Operation cancelled!", LogType.WARNING);
                break;
            case MUSIC_MODE:
                // file -> file
                var musicIn = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select rSoundAst file to decode from",
                    FileTypeFilter = new[] { musicType },
                    AllowMultiple = false
                });
                var musicOut = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Select audio file to decode to"
                });
                if (musicIn.Any() && musicOut != null)
                    ConversionPipeline.DecodeAST(musicIn[0], musicOut);
                else
                    Logger.Message("Operation cancelled!", LogType.WARNING);
                break;
        }
    }

    // create a file of Dead Rising format
    private async void Encode(object? sender, RoutedEventArgs e)
    {
        // just to be safe
        if (ModeTab == null)
            return;

        var topLevel = TopLevel.GetTopLevel(this);

        switch (ModeTab.SelectedIndex)
        {
            case SFX_MODE:
                // folder -> file
                var soundIn = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select folder to encode rSound from",
                    AllowMultiple = false
                });
                var soundOut = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Select rSound file to encode to",
                });
                if (soundIn.Any() && soundOut != null)
                    ConversionPipeline.EncodeSound(soundIn[0], soundOut);
                else
                    Logger.Message("Operation cancelled!", LogType.WARNING);
                break;
            case MUSIC_MODE:
                // file -> file
                var musicIn = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select audio file to encode from",
                    AllowMultiple = false
                });
                var musicOut = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Select AST file to encode to"
                });
                if (musicIn.Any() && musicOut != null)
                    ConversionPipeline.EncodeAST(musicIn[0], musicOut);
                else
                    Logger.Message("Operation cancelled!", LogType.WARNING);
                break;
        }
    }

    private void WriteLog(string log)
    {
        // https://docs.avaloniaui.net/docs/guides/development-guides/accessing-the-ui-thread
        Dispatcher.UIThread.Post(() =>
        {
            LogWindow.Text += log + Environment.NewLine;
        });
    }

    private void ModeTab_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        SetLabels();
    }

    private void ClearLog(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            LogWindow.Text = String.Empty;
        });
    }
}