namespace ASTRedux.Structs.CSound.Sub;

internal struct CSBHead
{
    public const int CSB_MAGIC = 0x20425343; // csb (CSoundBank?)
    public int CSBLength { get; set; } // (file length - csb offset) + 1
    public int NumSounds { get; set; } // number of sounds in file
}
