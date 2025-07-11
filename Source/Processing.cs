using System.Drawing;
using System.Linq;
using ASTRedux.Data.Format;
using ASTRedux.Data.RSound;
using ASTRedux.Data.RSound.Sub;
using ASTRedux.FileModels;
using ASTRedux.Utils;
using ASTRedux.Utils.Consts;
using ASTRedux.Utils.Helpers;
using ASTRedux.Utils.Logging;
using BinaryEx;
using ManagedBass;
using NaturalSort.Extension;


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
        int csbOffset = PositionReader.ReadInt32At(reader, Offset.pCSBPosition);
        Logger.Message($"CSB base address: 0x{csbOffset:x8}");
        int csbEntryOffset = PositionReader.ReadInt32At(reader, Offset.pCSBPosition) + 0x20;
        Logger.Message($"CSB entry base address: 0x{csbEntryOffset:x8}");

        int currEntry = 0;

        int audioCount = PositionReader.ReadInt32At(reader, csbOffset + 0x08);
        Logger.Message($"Audio count: 0x{audioCount:x8}");
        int audioStart = PositionReader.ReadInt32At(reader, csbOffset + 0x10) + csbOffset;
        Logger.Message($"Audio start: 0x{audioStart:x8}");

        int soundSize = 0;
        int soundOffset = 0;

        for (int i = 0; i < audioCount; i++)
        {
            string outName = String.Empty;

            currEntry = csbEntryOffset + 0x40 * i;
            Logger.Message($"Current CSB entry address: 0x{currEntry:x8}");

            SampleFormat csbFormat = new()
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
            Logger.Message($"Sound size: 0x{soundSize:x8}");
            Logger.Message($"Sound offset: 0x{soundOffset:x8} | Relative: 0x{PositionReader.ReadInt32At(reader, currEntry + 0x04):x8} + Absolute base: 0x{audioStart:x8}");

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
        using BinaryReader reader = new(File.OpenRead(output.FullName));
        using BinaryWriter writer = new(File.OpenWrite($"{output.FullName}.new"));

        // get data structure offsets from SNDL header
        int csbOffset = PositionReader.ReadInt32At(reader, Offset.pCSBPosition);
        int csbEntryOffset = PositionReader.ReadInt32At(reader, Offset.pCSBPosition) + 0x20;

        // fetch relative start of sound data from csb table, and then add offset of csb table to get absolute sound address
        int absoluteSndAddress = PositionReader.ReadInt32At(reader, csbOffset + 0x10) + csbOffset;

        Logger.Message($"CSB base structure offset: 0x{csbOffset:x8}");
        Logger.Message($"CSB entry base offset: 0x{csbEntryOffset:x8}");
        Logger.Message($"Absolute start address of audio data: 0x{absoluteSndAddress:x8}");

        // sound data containers for analyzeaudiodirectory
        List<byte[]> pcmBufs = [];
        List<SampleFormat> pcmFmts = [];

        // fetch pcm buffers, sound count, and cumulative buffer size
        AnalyzeAudioDirectory(input, out pcmBufs, out pcmFmts);

        // extract all header data from the rSound, including padding
        byte[] headBuf = PositionReader.ReadByteRange(
            reader,
            0x0,
            absoluteSndAddress);

        // write cumulative buffer sizes to tally total output file size
        headBuf.WriteInt32LE(Offset.FileSize, headBuf.Length + pcmBufs.Sum(pcm => pcm.Length));

        Logger.Message($"Data length {headBuf.Length + pcmBufs.Sum(pcm => pcm.Length)}");

        WriteCSB(headBuf, pcmBufs, pcmFmts, csbOffset, csbEntryOffset, absoluteSndAddress);

        writer.Seek(0, SeekOrigin.Begin);
        writer.Write(headBuf);

        writer.Seek(absoluteSndAddress, SeekOrigin.Begin);
        foreach (var pcm in pcmBufs)
        {
            writer.Write(pcm);
        }

        Logger.Message($"Audio write complete!");
    }

    private static void WriteCSB(byte[] headBuf, List<byte[]> pcmBufs, List<SampleFormat> pcmFmts, int csbOff, int csbEntryOff, int sndAddr)
    {
        int currEntryOff = 0;
        int sndCount = headBuf.ReadInt32LE(csbOff + Offset.csbSndNum);
        Logger.Message($"Sound count {sndCount} fetched");

        // relative offset of end of file to first csb entry
        headBuf.WriteInt32LE(csbOff + 0x04, pcmBufs.Sum(pcm => pcm.Length) + sndAddr - csbOff);
        Logger.Message($"End of file relative offset from CSB: 0x{pcmBufs.Sum(pcm => pcm.Length) + sndAddr - csbOff:x8}");

        // total audio data length
        headBuf.WriteInt32LE(csbOff + 0x14, pcmBufs.Sum(pcm => pcm.Length));
        Logger.Message($"0x{pcmBufs.Sum(pcm => pcm.Length):x8} PCM buffer summed length");

        for (int i = 0; i < sndCount; i++)
        {
            // get base address of current csb entry (if i is 0, this is just base + 0, so still first entry)
            currEntryOff = csbEntryOff + (0x40 * i);
            Logger.Message($"Current CSB entry begins at 0x{currEntryOff:x8}");

            headBuf.WriteInt32LE(currEntryOff, pcmBufs[i].Length);
            Logger.Message($"PCM Buffer Length 0x{pcmBufs[i].Length:X8}");
            headBuf.WriteInt32LE(currEntryOff + 0x04, CalculateOffset(i, pcmBufs));
            Logger.Message($"Relative PCM Offset 0x{CalculateOffset(i, pcmBufs):x8}");

            Logger.Message("Starting sample format write...");
            headBuf.WriteInt16LE(currEntryOff + 0x20, pcmFmts[i].FormatFlag);
            Logger.Message($"{pcmFmts[i].FormatFlag} format flag");
            headBuf.WriteInt16LE(currEntryOff + 0x22, pcmFmts[i].Channels);
            Logger.Message($"{pcmFmts[i].Channels} channels");
            headBuf.WriteInt32LE(currEntryOff + 0x24, pcmFmts[i].SampleRate);
            Logger.Message($"{pcmFmts[i].SampleRate}khz Sample Rate");
            headBuf.WriteInt32LE(currEntryOff + 0x28, pcmFmts[i].BytesPerSecond);
            Logger.Message($"{pcmFmts[i].BytesPerSecond} bytes per second");
            headBuf.WriteInt16LE(currEntryOff + 0x2C, pcmFmts[i].SampleSize);
            Logger.Message($"{pcmFmts[i].SampleSize} bytes per sample");
            headBuf.WriteInt16LE(currEntryOff + 0x2E, pcmFmts[i].BitDepth);
            Logger.Message($"{pcmFmts[i].BitDepth}-bit PCM");
        }
    }

    /// <summary>
    /// Returns the cumulative length in bytes of the raw PCM buffers of a directory of audio files, along with the individual lengths and number of files.
    /// </summary>
    /// <param name="dir">A DirectoryInfo object containing the directory to be scanned</param>
    /// <param name="eachLen">Every length in bytes of pcm buffer as list of ints</param>
    /// <param name="count">Number of files enumerated</param>
    /// <returns>Length in bytes of all PCM buffers</returns>
    public static void AnalyzeAudioDirectory(DirectoryInfo dir, out List<byte[]> pcmBufs, out List<SampleFormat> fmt)
    {
        pcmBufs = [];
        fmt = [];

        foreach (var file in dir.EnumerateFiles().OrderBy(x => x.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort()))
        {
            Logger.Message($"File {file.Name} being processed!");

            int streamHnd = Bass.CreateStream(file.FullName, 0, 0, BassFlags.Decode | BassFlags.Prescan);

            ChannelInfo ch = Bass.ChannelGetInfo(streamHnd);
            int totalLength = Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) <= int.MaxValue ? (int)Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) : 0;

            if (totalLength == 0)
                Logger.CriticalMessage("Audio length can not be determined!");

            byte[] pcmBuffer = new byte[totalLength];
            int bytesRead = Bass.ChannelGetData(streamHnd, pcmBuffer, totalLength);

            fmt.Add(new((short)ch.Channels, ch.Frequency, 16));

            pcmBufs.Add(pcmBuffer);

            Bass.StreamFree(streamHnd);
        }
    }
    /// <summary>
    /// Calculates the relative offset of an rSound wave block from absolute sound start address through cumulatively tallying all lengths prior.
    /// </summary>
    /// <param name="baseOffset">Offset of first byte of first audio chunk</param>
    /// <param name="count">What audio file are we processing?</param>
    /// <param name="lengths">Lengths of all audio files</param>
    /// <returns>Offset for the nth audio file, specified by count</returns>
    public static int CalculateOffset(int index, List<byte[]> pcmBufs) => pcmBufs.Take(index).Sum(len => len.Length);

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
