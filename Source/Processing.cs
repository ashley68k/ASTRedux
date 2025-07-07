using ASTRedux.FileModels;
using ASTRedux.Utils;
using ASTRedux.Utils.Helpers;
using ASTRedux.Utils.Logging;
using ManagedBass;

namespace ASTRedux;

internal static class Processing
{
    public static void ProcessAST(FileSystemInfo input, FileInfo output)
    {
        if (input is not FileInfo file) {
            Logger.CriticalMessage("Input isn't a file!");
            return;
        }

        Logger.Message("Input is valid!", LogType.INFO);

        using BinaryReader reader = new(file.OpenRead());
        using FileStream outStream = output.Create();

        if (!ASTFile.ValidateMagic(reader)) {
            Logger.CriticalMessage("Header doesn't match AST file!");
            return;
        }

        Logger.Message("AST magic matches!", LogType.INFO);

        ASTFile ast = new(reader, file.FullName);

        Logger.Message("AST header built!", LogType.INFO);

        reader.BaseStream.Position = ast.AudioInfo.StartOffset;
        byte[] outputBuffer = reader.ReadBytes(ast.AudioInfo.Length);

        Logger.Message("PCM buffer read from AST!", LogType.INFO);

        using WaveFileWriter memWriter = new(outStream, AudioHelpers.WaveFormatFromAudioFormat(ast.AudioInfo.Format));
        memWriter.Write(outputBuffer, outputBuffer.Length);

        Logger.Message("Buffer written to .wav!", LogType.INFO);

        Bass.Free();

        Logger.Message("BASS freed!", LogType.INFO);
    }

    public static void ProcessAudio(FileSystemInfo input, FileInfo output)
    {
        if(input is not FileInfo file) { 
            Logger.CriticalMessage("Input isn't a file!");
            return;
        }

        if (Config.OverwriteOutput)
            output.Delete();


        Logger.Message("Input is valid!", LogType.INFO);

        using BinaryWriter writer = new(File.OpenWrite(output.FullName));

        int streamHnd = Bass.CreateStream(file.FullName, 0, 0, BassFlags.Decode | BassFlags.Prescan);

        Logger.Message("BASS stream created!", LogType.INFO);

        if (streamHnd == 0) {
            Logger.CriticalMessage("Stream is null!");
            return;
        }

        ChannelInfo ch = Bass.ChannelGetInfo(streamHnd);
        int totalLength = Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) <= int.MaxValue ? (int)Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) : 0;

        if (totalLength == 0) {
            Logger.CriticalMessage("Audio length can not be determined!");
            return;
        }

        byte[] pcmBuffer = new byte[totalLength];
        int bytesRead = Bass.ChannelGetData(streamHnd, pcmBuffer, totalLength);

        if (bytesRead <= 0) {
            Logger.CriticalMessage("No bytes read to buffer!");
            return;
        }

        Logger.Message("Bytes read from BASS stream", LogType.INFO);

        ASTFile ast = new
        (
            new WaveFormat
            (
                ch.Frequency,
                16,
                ch.Channels
            ),
            totalLength
        );

        Logger.Message("AST object created!", LogType.INFO);

        writer.Write(ast.Header.ToByteArray());

        Logger.Message("Header written!", LogType.INFO);

        writer.BaseStream.Position = ast.AudioInfo.StartOffset;
        writer.Write(pcmBuffer);

        Logger.Message("PCM buffer written!", LogType.INFO);

        //Console.WriteLine($"Audio conversion to .ast finished! Audio duration is {AudioHelpers.GetAudioLength(ast.AudioInfo.Format.SampleRate, ast.AudioInfo.Format.BlockSize, ast.AudioInfo.Length):mm\\:ss\\.ff}.");

        Bass.Free();

        Logger.Message("BASS freed!", LogType.INFO);
    }
}
