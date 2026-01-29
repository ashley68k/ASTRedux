using ASTRedux.Utils.Logging;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace ASTRedux;
public partial class MainWindow : Window
{
    private const int SFX_MODE = 0;
    private const int MUSIC_MODE = 1;

    public MainWindow()
    {
        InitializeComponent();

        Logger.OnLogMessage += WriteLog;

        SetLabels();
    }

    private void SetLabels()
    {
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
    private void Decode(object? sender, RoutedEventArgs e)
    {
        switch (ModeTab.SelectedIndex)
        {
            case SFX_MODE:
                break;
            case MUSIC_MODE:
                break;
        }
    }

    // create a file of Dead Rising format
    private void Encode(object? sender, RoutedEventArgs e)
    {
        switch (ModeTab.SelectedIndex)
        {
            case SFX_MODE:
                break;
            case MUSIC_MODE:
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
}