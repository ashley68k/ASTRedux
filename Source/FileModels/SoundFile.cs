using ASTRedux.Data.CSound.Sub;
using ASTRedux.Utils;

namespace ASTRedux.FileModels;

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

    // data structures

    /*private CSRHead CSR_head;
    private List<CSREntry> CSR_entries;

    private CSHHead CSH_head;
    private List<CSHEntry> CSH_entries;

    private CSBHead CSB_head;
    private List<CSBEntry> CSB_entries;*/
}
