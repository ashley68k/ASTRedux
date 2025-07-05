using ManagedBass;

namespace ASTRedux.Utils;

internal static class PluginLoader
{
    public static void LoadPlugins(string rootDir)
    {
        // ugly linq hack to let plugin DLLs reside in the same directory as the bass.dll and dependency assemblies
        var files = Directory.EnumerateFiles($"{rootDir}", $"*.{OSUtils.LibraryExtension()}")
            .Where(file =>
                Path.GetFileNameWithoutExtension(file).Contains($"{OSUtils.GetBassLibraryName()}", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetFileNameWithoutExtension(file).Contains("ManagedBass", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Path.GetFileNameWithoutExtension(file), $"{OSUtils.GetBassLibraryName()}", StringComparison.OrdinalIgnoreCase)
            );

        foreach (var file in files)
        {
            using BinaryReader validator = new(File.OpenRead(file));

            // skip if magic doesn't match platform
            if (PositionReader.ReadUInt32At(validator, 0x00) != OSUtils.GetPEMagic())
            {
                Logger.Message($"Library PE magic {OSUtils.GetPEMagic()} match!", Enums.LogType.INFO);
                continue;
            }

            if(Bass.PluginLoad(file) == 0)
            {
                Logger.Message($"BASS Error '{Bass.LastError}' during plugin loading!\nPlugin Path: {file}", Enums.LogType.ERROR);
                break;
            }

            Logger.Message($"BASS plugin {Path.GetFileName(file)} loaded!", Enums.LogType.INFO);
        }
    }
}
