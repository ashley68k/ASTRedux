using ASTRedux.Data;
using ASTRedux.FileModels;
using ASTRedux.Utils;
using ManagedBass;

namespace ASTRedux;

internal static class Processing
{
    public static void ProcessAST(FileSystemInfo input, FileInfo output)
    {
        if (input is not FileInfo file)
        {
            throw new FileLoadException("Using directory input for AST is invalid!");
        }

        Logger.Message("Input is valid!", Enums.LogType.INFO);

        using BinaryReader reader = new(file.OpenRead());
        using FileStream outStream = output.Create();

        if (!ASTFile.ValidateMagic(reader))
        {
            Console.WriteLine("Header doesn't match AST file!");
            return;
        }

        Logger.Message("AST magic matches!", Enums.LogType.INFO);

        ASTFile ast = new(reader);

        Logger.Message("AST header built!", Enums.LogType.INFO);

        reader.BaseStream.Position = ast.AudioInfo.StartOffset;
        byte[] outputBuffer = reader.ReadBytes(ast.AudioInfo.Length);

        Logger.Message("PCM buffer read from AST!", Enums.LogType.INFO);

        using WaveFileWriter memWriter = new(outStream, AudioHelpers.WaveFormatFromAudioFormat(ast.AudioInfo.Format));
        memWriter.Write(outputBuffer, outputBuffer.Length);

        Logger.Message("Buffer written to .wav!", Enums.LogType.INFO);

        Bass.Free();
    }

    public static void ProcessAudio(FileSystemInfo input, FileInfo output)
    {
        if(input is not FileInfo file)
        {
            throw new FileLoadException("Using directory input for AST is invalid!");
        }

        Logger.Message("Input is valid!", Enums.LogType.INFO);

        using BinaryWriter writer = new(File.OpenWrite(output.FullName));

        int streamHnd = Bass.CreateStream(file.FullName, 0, 0, BassFlags.Decode | BassFlags.Prescan);

        Logger.Message("BASS stream created!", Enums.LogType.INFO);

        if (streamHnd == 0)
        {
            BassErrorHandler(file, Bass.LastError);
            return;
        }

        ChannelInfo ch = Bass.ChannelGetInfo(streamHnd);
        int totalLength = Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) <= int.MaxValue ? (int)Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) : 0;

        if (totalLength == 0)
        {
            BassErrorHandler(file, Bass.LastError);
            return;
        }

        byte[] pcmBuffer = new byte[totalLength];
        int bytesRead = Bass.ChannelGetData(streamHnd, pcmBuffer, totalLength);

        if (bytesRead <= 0)
        {
            BassErrorHandler(file, Bass.LastError);
            return;
        }

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

        writer.Write(ast.Header.ToByteArray());
        writer.BaseStream.Position = ast.AudioInfo.StartOffset;
        writer.Write(pcmBuffer);

        //Console.WriteLine($"Audio conversion to .ast finished! Audio duration is {AudioHelpers.GetAudioLength(ast.AudioInfo.Format.SampleRate, ast.AudioInfo.Format.BlockSize, ast.AudioInfo.Length):mm\\:ss\\.ff}.");

        Bass.Free();
    }

    private static void BassErrorHandler(FileSystemInfo input, Errors err)
    {
        switch (err)
        {
            // add more as needed/relevant
            case Errors.FileFormat:
                Logger.Message("File ", Enums.LogType.ERROR);
                break;
            default:
                Console.WriteLine($"BASS Error! Code #{err}");
                break;
        }
        Bass.Free();
        return;
    }
}
