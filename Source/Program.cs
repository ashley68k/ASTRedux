﻿using ASTRedux.Utils;
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
        if(OperatingSystem.IsWindows())
        { 
            PlatformHelpers.ValidateWin32CLI();
        }

        var inputOption = new Option<FileInfo>(
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

        var rootCommand = new RootCommand("ASTRedux");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);

        rootCommand.SetHandler((FileInfo input, FileInfo output) =>
        {
            if ((input != null) && (output != null))
            {
                FileValidate(input, output);
            }
        }, inputOption, outputOption);

        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Ran by the command line handler to validate and check the input file and pass command line args etc to where necessary.
    /// Actual processing code branches off for the sake of encapsulation, single responsibility, etc.
    /// </summary>
    /// <param name="input">A FileInfo object describing a file given on the CLI</param>
    /// <param name="output">A FileInfo object describing a file to be output on the CLI</param>
    private static void FileValidate(FileInfo input, FileInfo output)
    {
        if (!Bass.Init())
        {
            Console.WriteLine("Couldn't initialize BASS!");
        }

        if (input.FullName == output.FullName)
        {
            Console.WriteLine("Attempted output to input file!");
            return;
        }

        if (!input.Exists)
        {
            Console.WriteLine("Input file doesn't exist!");
            return;
        }
        if(output.Exists)
        {
            Console.WriteLine("Output file already exists!");
            return;
        }

        if (!string.IsNullOrEmpty(input.DirectoryName))
            PluginLoader.LoadPlugins(input.DirectoryName);
        else
            Console.WriteLine("No plugins to load!");

        // inputting AST will always result in a standard audio file output
        // inputting common audio file will always result in an AST output.
        if (FileExtensions.ASTExt.Contains(input.Extension) && output.Extension == ".wav")
        {
            Processing.ProcessAST(input, output);
        }
        else if (FileExtensions.ASTExt.Contains(output.Extension))
        {
            // let BASS handle input extension, as plugins can change support
            Processing.ProcessAudio(input, output);
        }
        else
        {
            Console.WriteLine($"Invalid output extension {output.Extension}. Try converting to wav!");
        }
        return;
    }
}
