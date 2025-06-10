using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTeroid.Structs;
using NAudio;
using NAudio.Wave;

namespace ASTeroid
{
    static class Processing
    {
        public static void ProcessAST(FileInfo input, FileInfo? output)
        {
            using var reader = new BinaryReader(input.OpenRead());

            if (!ASTFile.ValidateMagic(reader))
            {
                Console.WriteLine("Header doesn't match AST file!");
                return;
            }
        }

        public static void ProcessAudio(FileInfo input, FileInfo? output)
        {
            using var binaryReader = new BinaryReader(input.OpenRead());

            AudioFileReader audioInput = new(input.FullName);

            
        }
    }
}
