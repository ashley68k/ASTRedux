using System.Runtime.CompilerServices;
using ASTRedux.Data.AST;
using ASTRedux.Data.Format;
using ASTRedux.Utils;
using ManagedBass;

namespace ASTRedux.FileModels;

internal class ASTFile
{
    /// <summary>
    /// The magic number (ASTL) used by the AST format on LE platforms such as x86.
    /// Little Endian AST files are similar to .wav files, and use raw PCM. rSoundAst files in the .arc archive have much more complicated structures.
    /// Position 0x00 - 0x03
    /// </summary>
    public const int LITTLE_ENDIAN_MAGIC = 0x4C545341;
    /// <summary>
    /// The magic number (ASTB) used by the AST format on BE platforms such as PPC.
    /// BE support not implemented. Big Endian AST files, as in Dead Rising on the Xbox 360, use XMA encoding.
    /// Position 0x00 - 0x03
    /// </summary>
    public const int BIG_ENDIAN_MAGIC = 0x42545341;

    public ASTData AudioInfo { get; set; }

    public ASTHeader Header { get; set; }

    /// <summary>
    /// Compares the integer magic in an AST stream versus the expected magic
    /// </summary>
    /// <param name="reader">A BinaryReader containing the data of an AST file</param>
    /// <returns>True on LE/BE AST match, false otherwise</returns>
    public static bool ValidateMagic(BinaryReader reader, string filePath = "")
    {
        int streamMagic = PositionReader.ReadInt32At(reader, 0x00, filePath);
        return streamMagic == LITTLE_ENDIAN_MAGIC || streamMagic == BIG_ENDIAN_MAGIC;
    }

    /// <summary>
    /// Creates an instance of ASTFile given an audio input from BASS
    /// </summary>
    /// <param name="fmt">A ManagedBass WaveFormat object describing the format for the AST file to take on</param>
    /// <param name="byteLength">Length in bytes of the decoded audio buffer</param>
    public ASTFile(ManagedBass.WaveFormat fmt, int byteLength)
    {
        AudioInfo = new ASTData
        {
            StartOffset = 0x800,
            Length = byteLength,
            Format = new Data.Format.WaveFormat(
                (short)fmt.Encoding,
                (short)fmt.Channels,
                fmt.SampleRate,
                fmt.AverageBytesPerSecond,
                (short)fmt.BlockAlign,
                (short)fmt.BitsPerSample,
                0
            ),
        };
        Header = new ASTHeader(AudioInfo);
    }

    /// <summary>
    /// Creates an instance of ASTFile given an AST binaryreader input
    /// </summary>
    /// <param name="reader">A BinaryReader containing the AST binary data as a stream</param>
    public ASTFile(BinaryReader reader, string filePath = "")
    {
        AudioInfo = new ASTData
        {
            StartOffset = PositionReader.ReadInt32At(reader, 0x10, filePath),
            Length = PositionReader.ReadInt32At(reader, 0x20, filePath),
            Format = new Data.Format.WaveFormat(
                PositionReader.ReadInt16At(reader, 0x30, filePath), // format flag
                PositionReader.ReadInt16At(reader, 0x32, filePath), // channels
                PositionReader.ReadInt32At(reader, 0x34, filePath), // sample rate
                PositionReader.ReadInt32At(reader, 0x38, filePath), // bytes per second
                PositionReader.ReadInt16At(reader, 0x3C, filePath), // block size
                PositionReader.ReadInt16At(reader, 0x3E, filePath), // bit depth
                0
            ),
        };
        Header = new ASTHeader(AudioInfo);
    }
}
