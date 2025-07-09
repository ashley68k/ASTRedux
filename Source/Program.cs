using ASTRedux.Utils;
using ASTRedux.Utils.Consts;
using ASTRedux.Utils.Helpers;
using ASTRedux.Utils.Logging;
using ManagedBass;
using System.CommandLine;
using System.Reflection.Metadata.Ecma335;

namespace ASTRedux;

internal static class Program
{
    /// <summary>
    /// Entry point of the program, just parses CLI inputs
    /// </summary>
    /// <param name="args">CLI input</param>
    /// <returns>Invocation of command</returns>
    public static async Task<int> Main(string[] args)
    {
        var inputOption = new Option<FileSystemInfo>(
            name: "--input",
            description: "The file to be processed")
            {
                IsRequired = true
            };

        inputOption.AddAlias("-i");

        var outputOption = new Option<FileInfo>(
            name: "--output",
            description: "Filename to be output")
            {
                IsRequired = true
            };

        outputOption.AddAlias("-o");

        var overwrite = new Option<bool>(
            name: "--overwrite",
            description: "Overwrite output file",
            getDefaultValue: () => false
            );

        overwrite.AddAlias("-w");

        var verbosityLevel = new Option<LogDetail>(
            name: "--verbose",
            description: "Verbosity/logging level",
            getDefaultValue: () => LogDetail.NONE
            );

        verbosityLevel.AddAlias("-v");

        var rootCommand = new RootCommand("ASTRedux - a CLI tool to convert Dead Rising audio");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(verbosityLevel);
        rootCommand.AddOption(overwrite);

        rootCommand.SetHandler((FileSystemInfo input, FileInfo output, LogDetail level, bool overwrite) =>
        {
            Logger.VerbosityLevel = level;

            Config.OverwriteOutput = overwrite;

            if (level == LogDetail.EXTREME)
                Logger.SW.Start();

            if ((input != null) && (output != null))
            {
                Start(input, output);
            }
        }, inputOption, outputOption, verbosityLevel, overwrite);

        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Ran by the command line handler to validate and check the input file and pass command line args etc to where necessary.
    /// Actual processing code branches off for the sake of encapsulation, single responsibility, etc.
    /// </summary>
    /// <param name="input">A FileInfo object describing a file given on the CLI</param>
    /// <param name="output">A FileInfo object describing a file to be output on the CLI</param>
    private static void Start(FileSystemInfo input, FileInfo output)
    {
        if (!Validate(input, output))
            return;

        Logger.Message("All validations passed!", LogType.INFO);

        CheckForPlugins(input);

        Logger.Message("Plugin check complete!", LogType.INFO);

        SelectProcessingPipeline(input, output);

        if (Logger.VerbosityLevel != LogDetail.NONE)
            File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log", Logger.logOut.ToString());

        Logger.Message("Log file written!", LogType.INFO);

        return;
    }

    private static bool Validate(FileSystemInfo input, FileInfo output)
    {
        switch(input)
        {
            case FileInfo file:
                if (file.DirectoryName != null && !BASSHelpers.IsBassPresent(file.DirectoryName))
                {
                    Logger.CriticalMessage("BASS library doesn't exist!");
                    return false;
                }
                break;
            case DirectoryInfo dir:

                if(!dir.Exists)
                {
                    Logger.CriticalMessage("Input directory doesn't exist!");
                    return false;
                }

                if (dir.FullName != null && !BASSHelpers.IsBassPresent(dir.FullName))
                {
                    Logger.CriticalMessage("BASS library doesn't exist!");
                    return false;
                }

                break;
        }

        Logger.Message("BASS found!", LogType.INFO);

        if (!Bass.Init())
        {
            Logger.CriticalMessage("Couldn't initialize BASS!");
            return false;
        }

        Logger.Message("BASS initialized!", LogType.INFO);

        if (input.FullName == output.FullName)
        {
            Logger.CriticalMessage("Attempted to overwrite input!");
            return false;
        }

        Logger.Message("Overwrite check safe!", LogType.INFO);

        if (!input.Exists)
        {
            Logger.CriticalMessage("Input doesn't exist!");
            return false;
        }

        Logger.Message("Input exists!", LogType.INFO);

        if (output.Exists && !Config.OverwriteOutput)
        {
            Logger.CriticalMessage("Output already exists!");
            return false;
        }

        Logger.Message(Config.OverwriteOutput ? "Overwriting output file!" : "Output file doesn't exist!", LogType.INFO);

        return true;
    }

    private static void SelectProcessingPipeline(FileSystemInfo input, FileSystemInfo output)
    {
        Logger.Message("Processing branch reached!", LogType.INFO);

        SoundType type = GetProcessType(input, output);

        if(type == SoundType.INVALID)
        {
            Logger.CriticalMessage("Process type can't be determined!");
            return;
        }

        switch (type)
        {
            // i cast input/output to their respective types here. this could theoretically be risky,
            // however, i already validated it in GetProcessType(). this just avoids redundant checking later, and is
            // further guarded by the above invalid check.
            case SoundType.AST_IN:
                Logger.Message($"File {input.Extension} -> {output.Extension} path, run ProcessAST()", LogType.INFO);
                Processing.ProcessAST(input as FileInfo, output as FileInfo);
                break;
            case SoundType.AST_OUT:
                Logger.Message($"File {input.Extension} -> {output.Extension} path, run ProcessMusic()", LogType.INFO);
                Processing.ProcessMusic(input as FileInfo, output as FileInfo);
                break;
            case SoundType.SOUND_IN:
                Logger.Message($"File {input.Extension} -> {output.FullName} path, run ProcessSoundIn()", LogType.INFO);
                Processing.ProcessSoundIn(input as FileInfo, output as DirectoryInfo);
                break;
            case SoundType.SOUND_OUT:
                Logger.Message($"File {input.Extension} -> {output.FullName} path, run ProcessSoundOut()", LogType.INFO);
                Processing.ProcessSoundOut(input as DirectoryInfo, output as FileInfo);
                break;
        }
    }

    private static void CheckForPlugins(FileSystemInfo input)
    {
        switch(input)
        {
            case FileInfo inFile:
                if (!string.IsNullOrEmpty(inFile.DirectoryName))
                    PluginLoader.LoadPlugins(inFile.DirectoryName);
                    break;
            case DirectoryInfo inDir:
                if (!string.IsNullOrEmpty(inDir.FullName))
                    PluginLoader.LoadPlugins(inDir.FullName);
                    break;
        }
    }

    private static SoundType GetProcessType(FileSystemInfo input, FileSystemInfo output)
    {
        return (input, output) switch
        {
            (FileInfo inFile, FileInfo outFile) =>
                FileExtensions.ASTExt.Contains(inFile.Extension) && outFile.Extension == ".wav"
                    ? SoundType.AST_IN
                    : FileExtensions.ASTExt.Contains(outFile.Extension)
                        ? SoundType.AST_OUT
                        : SoundType.INVALID,

            (FileInfo inFile, DirectoryInfo outDir) =>
                !outDir.Exists && FileExtensions.SoundExt.Contains(inFile.Extension)
                    ? SoundType.SOUND_IN
                    : SoundType.INVALID,

            (DirectoryInfo inDir, DirectoryInfo outFile) =>
                inDir.Exists && FileExtensions.SoundExt.Contains(outFile.Extension)
                    ? SoundType.SOUND_OUT
                    : SoundType.INVALID,

            _ => SoundType.INVALID
        };
    }
}
