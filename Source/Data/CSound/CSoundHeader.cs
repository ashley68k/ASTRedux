using ASTRedux.Utils;

namespace ASTRedux.Data.CSound;

// 0x20 length
internal struct CSoundHeader
{
    public const int FILE_MAGIC = 0x4C444E53; // sndl (sound, little endian)
    public int FileLength { get; private set; } // file length + 1
    
    public int Unknown1 { get; private set; } // 512?

    public uint FFPad_1 = Constants.FFFF_PAD;

    public int CSROffset { get; private set; }

    public int CSHOffset { get; private set; }
    public int CSBOffset { get; private set; }

    public uint FFPad_2 = Constants.FFFF_PAD;

    public CSoundHeader()
    {
    }
}
