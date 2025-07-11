using System.Runtime.InteropServices;
using ASTRedux.Utils.Consts;

namespace ASTRedux.Data.RSound;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct RSoundHeader(int len, int csrOff, int cshOff, int csbOff)
{
    public readonly int FILE_MAGIC = 0x4C444E53; // sndl (sound, little endian)
    public int FileLength { get; private set; } = len;

    public readonly int Unknown1 = 512; // 512?

    public readonly uint FFPad_1 = Constants.FFFF_PAD;

    public int CSROffset { get; private set; } = csrOff;

    public int CSHOffset { get; private set; } = cshOff;
    public int CSBOffset { get; private set; } = csbOff;

    public readonly uint FFPad_2 = Constants.FFFF_PAD;
}
