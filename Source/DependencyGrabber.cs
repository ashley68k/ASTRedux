using ASTRedux.Utils;
using ASTRedux.Utils.Logging;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ASTRedux;

internal static class DependencyGrabber
{
    private static readonly string bassFile = $"bass24{OSUtils.GetBassDownloadSuffix()}.zip";

    /// <summary>
    /// A generic async downloader to be used for dependency acquisition for convenience, 
    /// license compliance, and graceful error handling.
    /// </summary>
    /// <param name="baseUrl">The url to the file server, including directory structure.</param>
    /// <param name="fileName">The filename of the file to retrieve from the server and output to</param>
    /// <returns>A bool representing success of method. If true, the existence of fileName should be guaranteed.</returns>
    private static async Task<bool> DownloadAsync(string baseUrl, string fileName)
    {
        if (Path.Exists(Path.Combine(AppContext.BaseDirectory, fileName)) && !Config.OverwriteOutput)
        {
            Logger.CriticalMessage("Download target already exists!");
            return false;
        }

        string finalUrl = Path.Combine(baseUrl, fileName);
        using HttpClient http = new();

        try
        {
            Logger.Message($"Attempting to fetch URL {finalUrl}");
            using var download = await http.GetStreamAsync(finalUrl);

            try
            {
                using var outzip = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                await download.CopyToAsync(outzip);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                Logger.CriticalMessage($"Access denied writing '{fileName}': {uaEx.Message}");
                return false;
            }
            catch (IOException ioEx)
            {
                Logger.CriticalMessage($"'{fileName}' creation failed with error {ioEx.Message}");
                return false;
            }

            return true;
        }
        catch (HttpRequestException httpEx)
        {
            Logger.CriticalMessage($"BASS library acquisition failed with HTTP response {httpEx.StatusCode}");
            return false;
        }
        catch (InvalidOperationException invopEx)
        {
            Logger.CriticalMessage($"Invalid operation {invopEx.Message} performed while downloading {fileName}");
            return false;
        }
    }

    public static async Task GrabBassLibrary()
    {
        using HttpClient http = new();

        string bassLibUrl = $"https://www.un4seen.com/files/";

        // Logger.Message(Path.Combine(AppContext.BaseDirectory, $"{OSUtils.GetBassLibraryName() + '.' + OSUtils.LibraryExtension()}"));
        
        if (Path.Exists(Path.Combine(AppContext.BaseDirectory, $"{OSUtils.GetBassLibraryName() + '.' + OSUtils.LibraryExtension()}")))
        {
            Logger.Message($"BASS exists!");
            return;
        }

        if (!await DownloadAsync(bassLibUrl, bassFile))
        {
            Logger.CriticalMessage("BASS acquisition failed!");
            return;
        }

        Logger.Message("BASS library downloaded!");

        using ZipArchive arc = ZipFile.OpenRead(bassFile);
        {
            // zip spec specifies forward slashes for paths
            string attemptPath = Path.Combine($"{OSUtils.GetBassZipPath()}", $"{OSUtils.GetBassLibraryName()}.{OSUtils.LibraryExtension()}").Replace("\\", "/");
            try
            {
                var filterZip = arc.Entries.Single(entry => entry.FullName
                    == attemptPath);

                filterZip.ExtractToFile(Path.Combine(AppContext.BaseDirectory, filterZip.Name));

                Logger.Message("BASS library extracted!");
            }
            catch (InvalidOperationException invopEx)
            {
                Logger.CriticalMessage($"Invalid operation on {bassFile}: {invopEx.Message} attempted path {attemptPath}");
                return;
            }
        }

        arc.Dispose();

        Logger.Message($"Deleting temp archive {bassFile}");

        if (!string.IsNullOrEmpty(bassFile) && Path.Exists(bassFile))
            System.IO.File.Delete(bassFile);
        else
            Logger.CriticalMessage("Attempted to delete a NULL file!");

            Logger.Message($"BASS installation finished!\n\n" +
                $"You can install BASS plugins from https://www.un4seen.com/\nby selecting the plugin's {OSUtils.GetOSString()} download.\n" +
                $"Simply put the plugin's x64 .{OSUtils.LibraryExtension()} file\nin the root directory of your ASTRedux installation.\n" +
                $"Exiting in 10 seconds!\nPlease relaunch ASTRedux!");

        await Task.Delay(10000);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}