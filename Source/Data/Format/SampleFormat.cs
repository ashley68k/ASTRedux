using System.Runtime.InteropServices;

namespace ASTRedux.Data.Format;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct SampleFormat
{
    /// <summary>
    /// NOTE: Game executable appears to discard the format flag for BGM audio, which will always be read as 1 (PCM). Cutscene audio appears to be read as 0xFFFE, extended.
    /// cbSize is always zero in PCM files, and in cutscene/event AST files, is 22, the minimum value for 0xFFFE blocks. 
    /// </summary>
    public short FormatFlag { get; set; }
    public short Channels { get; set; }
    public int SampleRate { get; set; }
    public int BytesPerSecond { get; set; }
    public short SampleSize { get; set; }
    public short BitDepth { get; set; }

    public SampleFormat(short formatFlag, short channels, int sampleRate, int bps, short sampleSize, short bitDepth)
    {
        FormatFlag = formatFlag;
        Channels = channels;
        SampleRate = sampleRate;
        BytesPerSecond = bps;
        SampleSize = sampleSize;
        BitDepth = bitDepth;
    }
    public SampleFormat(short channels, int sampleRate, short bitDepth)
    {
        FormatFlag = 1;
        Channels = channels;
        SampleRate = sampleRate;
        BytesPerSecond = (sampleRate * bitDepth * channels) / 8;
        SampleSize = (short)((channels * bitDepth) / 8);
        BitDepth = bitDepth;
    }
}
