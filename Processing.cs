using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            using BinaryReader reader = new BinaryReader(input.OpenRead());
            
            if (!ASTFile.ValidateMagic(reader))
            {
                Console.WriteLine("Header doesn't match AST file!");
                return;
            }

            ASTFile ast = new ASTFile(
                ASTFile.ParseData(reader)
            );

            reader.BaseStream.Position = ast.AudioInfo.StartOffset;
            byte[] outputBuffer = reader.ReadBytes(ast.AudioInfo.Length);

            WaveFormat outFormat = WaveFormat.CreateCustomFormat(
                WaveFormatEncoding.Pcm, 
                ast.AudioInfo.SampleRate, 
                ast.AudioInfo.Channels, 
                ast.AudioInfo.BytesPerSecond, 
                ast.AudioInfo.BlockSize,
                ast.AudioInfo.BitDepth
            );

            using WaveFileWriter writer = new(File.OpenWrite(output.FullName), outFormat);

            writer.Write(outputBuffer);

            Console.WriteLine("Audio conversion from .ast finished!");
        }

        public static void ProcessAudio(FileInfo input, FileInfo? output)
        {
            using AudioFileReader audioInput = new(input.FullName);
            using BinaryReader reader = new BinaryReader(input.OpenRead());
            using BinaryWriter writer = new BinaryWriter(File.OpenWrite(output.FullName));

            using var pcmStream = new Wave32To16Stream(audioInput);
            using var ms = new MemoryStream();
            pcmStream.CopyTo(ms);
            byte[] pcmBuffer = ms.ToArray();

            ASTFile ast = new(
                ASTFile.ParseData(pcmStream, pcmBuffer.Length)
            );

            ASTHeader header = new ASTHeader(ast.AudioInfo);

            writer.Write(header.ToByteArray());
            writer.BaseStream.Position = ast.AudioInfo.StartOffset;
            writer.Write(pcmBuffer);

            Console.WriteLine("Audio conversion to .ast finished!");
        }
    }
}
