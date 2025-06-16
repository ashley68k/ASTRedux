using System.Runtime.InteropServices;

namespace ASTRedux.Structs.Format;

// directly analogous to WAVEFORMAT
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct AudioFormat(short formatFlag, short channels, int sampleRate, int bps, short blockSize, short bitDepth)
{
    public short FormatFlag { get; set; } = formatFlag;
    public short Channels { get; set; } = channels;
    public int SampleRate { get; set; } = sampleRate;
    public int BytesPerSecond { get; set; } = bps;
    public short BlockSize { get; set; } = blockSize;
    public short BitDepth { get; set; } = bitDepth;
}
