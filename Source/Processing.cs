﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ASTRedux.Structs;
using ASTRedux.Structs.AST;
using NAudio;
using NAudio.Wave;

namespace ASTRedux
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
                (WaveFormatEncoding)ast.AudioInfo.Format.FormatFlag, 
                ast.AudioInfo.Format.SampleRate, 
                ast.AudioInfo.Format.Channels, 
                ast.AudioInfo.Format.BytesPerSecond, 
                ast.AudioInfo.Format.BlockSize,
                ast.AudioInfo.Format.BitDepth
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

            ast.Header = new ASTHeader(ast.AudioInfo);

            writer.Write(ast.Header.ToByteArray());
            writer.BaseStream.Position = ast.AudioInfo.StartOffset;
            writer.Write(pcmBuffer);

            Console.WriteLine("Audio conversion to .ast finished!");
        }
    }
}
