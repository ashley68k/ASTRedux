using ASTRedux.Enums;
using ASTRedux.Utils;
using ManagedBass;
using System.CommandLine;

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

        var outputOption = new Option<FileInfo>(
            name: "--output",
            description: "Filename to be output")
            {
                IsRequired = true
            };

        var verbosity = new Option<bool>(
            name: "--verbose",
            description: "Output detailed log messages",
            getDefaultValue: () => false
            );

        var verbosityLevel = new Option<LogLevel>(
            name: "--level",
            description: "Verbosity level",
            getDefaultValue: () => 0
            );


        var rootCommand = new RootCommand("ASTRedux");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(verbosity);
        rootCommand.AddOption(verbosityLevel);

        rootCommand.SetHandler((FileSystemInfo input, FileInfo output, bool verbose, LogLevel level) =>
        {
            Logger.IsVerbose = verbose;
            Logger.Level = level;

            if (level == LogLevel.EXTREME)
                Logger.SW.Start();

            if ((input != null) && (output != null))
            {
                Start(input, output);
            }
        }, inputOption, outputOption, verbosity, verbosityLevel);

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

        Logger.Message("All validations passed!", Enums.LogType.INFO);

        CheckForPlugins(input);

        Logger.Message("Plugin check complete!", Enums.LogType.INFO);

        try { 
            SelectProcessingPipeline(input, output);
        }
        catch(Exception ex) {
            Console.WriteLine($"\n{ex}");
        }
        return;
    }

    private static bool Validate(FileSystemInfo input, FileInfo output)
    {
        switch(input)
        {
            case FileInfo file:
                if (file.DirectoryName != null && !BASSHelpers.IsBassPresent(file.DirectoryName))
                {
                    Console.WriteLine("BASS library doesn't exist!");
                    return false;
                }
                break;
            case DirectoryInfo dir:
                if (dir.FullName != null && !BASSHelpers.IsBassPresent(dir.FullName))
                {
                    Console.WriteLine("BASS library doesn't exist!");
                    return false;
                }
                break;
        }

        Logger.Message("BASS found!", Enums.LogType.INFO);

        if (!Bass.Init())
        {
            Logger.CriticalMessage("Couldn't initialize BASS!");
            return false;
        }

        Logger.Message("BASS initialized!", Enums.LogType.INFO);

        if (input.FullName == output.FullName)
        {
            Logger.CriticalMessage("Attempted to overwrite input!");
            return false;
        }

        Logger.Message("Overwrite check safe!", Enums.LogType.INFO);

        if (!input.Exists)
        {
            Logger.CriticalMessage("Input doesn't exist!");
            return false;
        }

        Logger.Message("Input exists!", Enums.LogType.INFO);

        if (output.Exists)
        {
            Logger.CriticalMessage("Output already exists!");
            return false;
        }

        Logger.Message("Output doesn't exist!", Enums.LogType.INFO);

        return true;
    }

    private static void SelectProcessingPipeline(FileSystemInfo input, FileInfo output)
    {
        Logger.Message("Processing branch reached!", Enums.LogType.INFO);
        switch (input)
        {
            case FileInfo file:
                if (FileExtensions.ASTExt.Contains(file.Extension) && output.Extension == ".wav")
                {
                    Processing.ProcessAST(input, output);
                    Logger.Message($"File {file.Extension} -> {output.Extension} path, run ProcessAST", Enums.LogType.INFO);
                }
                else if (FileExtensions.ASTExt.Contains(output.Extension))
                {
                    // let BASS handle input extension, as plugins can change support
                    Processing.ProcessAudio(input, output);
                    Logger.Message($"File {file.Extension} -> {output.Extension} path, run ProcessAudio", Enums.LogType.INFO);
                }
                else
                {
                    Logger.CriticalMessage($"File {file.Extension} -> {output.Extension} path has nowhere to go!");
                    throw new InvalidDataException("Invalid extension! Are you missing a BASS plugin, omitting extension, or exporting to non-wav file?");
                }
                break;
            case DirectoryInfo dir:
                Logger.CriticalMessage("Input interpreted as directory!");
                throw new NotImplementedException("rSound conversion/multi-processing not yet implemented!");
        }
    }

    private static void CheckForPlugins(FileSystemInfo input)
    {
        switch(input)
        {
            case FileInfo inFile:
                if (!string.IsNullOrEmpty(inFile.DirectoryName))
                    PluginLoader.LoadPlugins(inFile.DirectoryName);
                else
                    Console.WriteLine("No plugins to load!");
                break;
            case DirectoryInfo inDir:
                if (!string.IsNullOrEmpty(inDir.FullName))
                    PluginLoader.LoadPlugins(inDir.FullName);
                else
                    Console.WriteLine("No plugins to load!");
                break;
        }
    }
}
