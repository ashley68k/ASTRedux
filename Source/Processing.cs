using ASTRedux.Data.Format;
using ASTRedux.Data.RSound;
using ASTRedux.Data.RSound.Sub;
using ASTRedux.FileModels;
using ASTRedux.Utils;
using ASTRedux.Utils.Helpers;
using ASTRedux.Utils.Logging;
using ManagedBass;

namespace ASTRedux;

internal static class Processing
{
    public static void ProcessAST(FileInfo input, FileInfo output)
    {
        Logger.Message("Input is valid!", LogType.INFO);

        using BinaryReader reader = new(input.OpenRead());
        using FileStream outStream = output.Create();

        if (!ASTFile.ValidateMagic(reader)) {
            Logger.CriticalMessage("Header doesn't match AST file!");
            return;
        }

        Logger.Message("AST magic matches!", LogType.INFO);

        ASTFile ast = new(reader, input.FullName);

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

    public static void ProcessMusic(FileInfo input, FileInfo output)
    {
        if (Config.OverwriteOutput)
            output.Delete();

        Logger.Message("Input is valid!", LogType.INFO);

        using BinaryWriter writer = new(File.OpenWrite(output.FullName));

        int streamHnd = Bass.CreateStream(input.FullName, 0, 0, BassFlags.Decode | BassFlags.Prescan);

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

    public static void ProcessSoundIn(FileInfo input, DirectoryInfo output)
    {
        using BinaryReader reader = new(File.OpenRead(input.FullName));

        if (output.Exists && Config.OverwriteOutput)
        {
            if (!OverwritePrompt(output))
                return;
        }

        if (!output.Exists)
            output.Create();

        // read csb start offset from file, get audio count from it, then copy into buffer
        int csbStart = PositionReader.ReadInt32At(reader, 0x18);

        int firstEntry = csbStart + PositionReader.ReadInt32At(reader, csbStart + 0x0C);
        int currEntry = 0;

        int audioCount = PositionReader.ReadInt32At(reader, csbStart + 0x08);
        int audioStart = PositionReader.ReadInt32At(reader, csbStart + 0x10);

        int soundSize = 0;
        int soundOffset = 0;

        for (int i = 0; i < audioCount; i++)
        {
            string outName = String.Empty;

            currEntry = firstEntry + 0x40 * i;

            SampleFormat csbFormat = new SampleFormat
            {
                FormatFlag = PositionReader.ReadInt16At(reader, currEntry + 0x20),
                Channels = PositionReader.ReadInt16At(reader, currEntry + 0x22),
                SampleRate = PositionReader.ReadInt32At(reader, currEntry + 0x24),
                BytesPerSecond = PositionReader.ReadInt32At(reader, currEntry + 0x28),
                SampleSize = PositionReader.ReadInt16At(reader, currEntry + 0x2C),
                BitDepth = PositionReader.ReadInt16At(reader, currEntry + 0x2E),
            };

            soundSize = PositionReader.ReadInt32At(reader, currEntry + 0x00);
            soundOffset = PositionReader.ReadInt32At(reader, currEntry + 0x04) + audioStart;

            reader.BaseStream.Position = soundOffset;
            byte[] outBuf = reader.ReadBytes(soundSize);

            outName = Path.Combine($"{output.FullName}", $"{i+1}.wav");

            using FileStream outStream = File.Create(outName);

            using WaveFileWriter memWriter = new(outStream, AudioHelpers.WaveFormatFromAudioFormat(csbFormat));
            memWriter.Write(outBuf, outBuf.Length);

            Logger.Message($"File {outName} written!");
        }

        Bass.Free();

        return;
    }

    public static void ProcessSoundOut(DirectoryInfo input, FileInfo output)
    {
        // stub for now
    }

    // prevent an rm -rf moment
    private static bool OverwritePrompt(DirectoryInfo dir)
    {
        Console.WriteLine($"Are you sure you want to recursively delete and overwrite directory {dir.FullName}? (Y/N)");
        char sel = char.ToLower((char)Console.Read());
        switch (sel)
        {
            case 'y':
                Console.WriteLine("Deleting...");
                dir.Delete(true);
                return true;
            case 'n':
                Console.WriteLine("Exiting!");
                return false;
            default:
                Console.WriteLine("Invalid choice!");
                return false;
        }
    }
}
