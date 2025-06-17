using ManagedBass;

namespace ASTRedux.Utils;

internal static class PluginLoader
{
    public static void LoadPlugins(string rootDir)
    {
        // ugly linq hack to let plugin DLLs reside in the same directory as the bass.dll and dependency assemblies
        var files = Directory.EnumerateFiles($"{rootDir}", "*.dll")
            .Where(file =>
                Path.GetFileNameWithoutExtension(file).Contains("bass", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetFileNameWithoutExtension(file).Contains("ManagedBass", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Path.GetFileNameWithoutExtension(file), "bass", StringComparison.OrdinalIgnoreCase)
            );

        foreach (var file in files)
        {
            using BinaryReader validator = new(File.OpenRead(file));

            // skip if magic isn't MZ
            if (PositionReader.ReadInt16At(validator, 0x00) != 0x5A4D)
                continue;

            // TODO: do more rigorous testing such as substring searches for BASSplugin export etc.

            if(Bass.PluginLoad(file) == 0)
            {
                Console.WriteLine($"BASS Error '{Bass.LastError}' during plugin loading!\nPlugin Path: {file}");
                break;
            }

            Console.WriteLine($"BASS plugin {file} loaded!");
        }
    }
}
