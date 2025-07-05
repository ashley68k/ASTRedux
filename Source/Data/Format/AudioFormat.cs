using System.Runtime.InteropServices;

namespace ASTRedux.Data.Format;

// directly analogous to WAVEFORMAT
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct AudioFormat(short formatFlag, short channels, int sampleRate, int bps, short blockSize, short bitDepth)
{
    /// <summary>
    /// NOTE: Game executable appears to discard the format flag, and always read as though it were PCM (1) only.
    /// Don't go trying to import float or ADPCM data or whatnot, it will just be read as PCM.
    /// </summary>
    public short FormatFlag { get; set; } = formatFlag;
    public short Channels { get; set; } = channels;
    public int SampleRate { get; set; } = sampleRate;
    public int BytesPerSecond { get; set; } = bps;
    public short BlockSize { get; set; } = blockSize;
    public short BitDepth { get; set; } = bitDepth;
}
