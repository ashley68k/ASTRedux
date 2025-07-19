using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using ASTRedux.Utils.Consts;

namespace ASTRedux.Data.RSound.Sub;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct CSBHead(int dataSize, int waveCnt, int csbStart, int audioStart, int audioSize)
{
    public readonly int CSB_MAGIC = 0x20425343; // csb (CSoundBank?)

    // file end - csb head start
    public int DataSize = dataSize;

    public int WaveCount = waveCnt;

    // csb head + csb entry start = 1st byte of 1st entry
    public int CSBEntryStart = csbStart;

    // audio offset - csb head start
    public int AudioStart = audioStart;

    // file end - audio start
    public int TotalAudioSize = audioSize;

    // always 1?
    public int Unknown = 1;

    public uint FFPAD = Constants.FFFF_PAD;
}
