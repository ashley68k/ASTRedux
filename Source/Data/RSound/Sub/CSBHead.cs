using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using ASTRedux.Utils.Consts;

namespace ASTRedux.Data.RSound.Sub;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct CSBHead()
{
    public const int CSB_MAGIC = 0x20425343; // csb (CSoundBank?)

    // file end - csb head start
    public int DataSize;

    public int WaveCount;

    // csb head + csb entry start = 1st byte of 1st entry
    public int CSBEntryStart;

    // audio offset - csb head start
    public int AudioStart;

    // file end - audio start
    public int TotalAudioSize;

    // always 1?
    public int Unknown = 1;

    public uint FFPAD = Constants.FFFF_PAD;
}
