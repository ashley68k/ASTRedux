using ASTRedux.Data.Format;
using ASTRedux.Utils.Consts;

namespace ASTRedux.Data.RSound.Sub;

// always padded to 0x40
internal struct CSBEntry(WaveFormat fmt, int dataSize = 0, int dataOffset = 0)
{
    // size in bytes of the sound entry
    public int SoundDataSize = dataSize;

    // relative to where sound data starts
    public int SoundDataOffset = dataOffset;

    // always 6???
    public int Unknown = 6;

    public uint FFPAD1 = Constants.FFFF_PAD;
    public uint FFPAD2 = Constants.FFFF_PAD;
    public uint FFPAD3 = Constants.FFFF_PAD;
    public uint FFPAD4 = Constants.FFFF_PAD;
    public uint FFPAD5 = Constants.FFFF_PAD;
    public WaveFormat Format { get; private set; }

    public uint FFPAD6 = Constants.FFFF_PAD;
    public uint FFPAD7 = Constants.FFFF_PAD;
    public uint FFPAD8 = Constants.FFFF_PAD;
    public uint FFPAD9 = Constants.FFFF_PAD;
}
