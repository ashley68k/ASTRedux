using ASTRedux.Structs;
using ASTRedux.Utils;
using System.CommandLine;
using System.Diagnostics;

namespace ASTRedux
{
    internal class Program
    {
        /// <summary>
        /// Entry point of the program, just parses CLI inputs
        /// </summary>
        /// <param name="args">CLI input</param>
        /// <returns>Invocation of command</returns>
        static async Task<int> Main(string[] args)
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
        static void FileValidate(FileInfo input, FileInfo output)
        {
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

            // inputting AST will always result in a standard audio file output
            // inputting common audio file will always result in an AST output.
            if (FileExtensions.ASTExt.Contains(input.Extension) && FileExtensions.OutputExt.Contains(output.Extension))
            {
                Processing.ProcessAST(input, output);
            }
            else if (FileExtensions.InputExt.Contains(input.Extension) && FileExtensions.ASTExt.Contains(output.Extension))
            {
                Processing.ProcessAudio(input, output);
            }
            else 
            {
                Console.WriteLine($"Invalid conversion from '{input.Extension}' to '{output.Extension}'");
            }
            return;
        }
    }
}
