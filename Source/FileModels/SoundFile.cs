using System.Runtime.InteropServices;
using ASTRedux.Data.RSound;
using ASTRedux.Data.RSound.Sub;
using ASTRedux.Utils;

namespace ASTRedux.FileModels;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal class SoundFile
{
    /* SNDL */
    public const int LITTLE_ENDIAN_MAGIC = 0x4C444E53;

    /* SNDP */
    public const int BIG_ENDIAN_MAGIC = 0x42444E53;

    public static bool ValidateMagic(BinaryReader reader)
    {
        int streamMagic = PositionReader.ReadInt32At(reader, 0x00);
        return streamMagic == LITTLE_ENDIAN_MAGIC || streamMagic == BIG_ENDIAN_MAGIC;
    }

    RSoundHeader RSoundHeader = new RSoundHeader();

    // data structures

    /*public CSRHead CSR_head;
    public List<CSREntry> CSR_entries;

    public CSHHead CSH_head;
    public List<CSHEntry> CSH_entries;*/

    public CSBHead CSB_head;
    public List<CSBEntry> CSB_entries;

    public required List<byte[]> pcmBuffers;
}
