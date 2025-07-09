using System.Runtime.InteropServices;

namespace ASTRedux.Data.RSound.Sub;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct CSHHead
{
    public const int CSH_MAGIC = 0x20485343; // csh (CSoundHeader?)
}
