using System.Runtime.InteropServices;

namespace ASTRedux.Data.Format;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct SampleFormat(short formatFlag, short channels, int sampleRate, int bps, short sampleSize, short bitDepth, short cbSize)
{
    /// <summary>
    /// NOTE: Game executable appears to discard the format flag for BGM audio, which will always be read as 1 (PCM). Cutscene audio appears to be read as 0xFFFE, extended.
    /// cbSize is always zero in PCM files, and in cutscene/event AST files, is 22, the minimum value for 0xFFFE blocks. 
    /// </summary>
    public short FormatFlag { get; set; } = formatFlag;
    public short Channels { get; set; } = channels;
    public int SampleRate { get; set; } = sampleRate;
    public int BytesPerSecond { get; set; } = bps;
    public short SampleSize { get; set; } = sampleSize;
    public short BitDepth { get; set; } = bitDepth;

    public short CustomBlockSize { get; set; } = cbSize;
}
