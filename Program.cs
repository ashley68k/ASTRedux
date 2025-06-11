using ASTeroid.Structs;
using System.CommandLine;

namespace ASTeroid
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
            var inputOption = new Option<FileInfo?>(
                name: "--input",
                description: "The file to be processed");

            var outputOption = new Option<FileInfo?>(
                name: "--output",
                description: "Filename to be output");

            var rootCommand = new RootCommand("ASTeroid");
            rootCommand.AddOption(inputOption);
            rootCommand.AddOption(outputOption);

            rootCommand.SetHandler((FileInfo? input, FileInfo? output) =>
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
        static void FileValidate(FileInfo input, FileInfo? output)
        {
            if (!input.Exists)
            {
                Console.WriteLine("Input file doesn't exist!");
                return;
            }
            if(output.Exists)
            {
                Console.WriteLine("Output file already exists! Deleting old!");
                output.Delete();
            }

            // inputting AST will always result in a standard audio file output
            // inputting common audio file will always result in an AST output.
            if (Extensions.ASTExt.Contains(input.Extension))
            {
                Processing.ProcessAST(input, output);
            }
            else if (Extensions.CommonExt.Contains(input.Extension))
            {
                Processing.ProcessAudio(input, output);
            }
            else 
            {
                Console.WriteLine($"Extension input {input.Extension} is invalid!");
            }
            return;
        }
    }
}
