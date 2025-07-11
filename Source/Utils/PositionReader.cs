using ASTRedux.Utils.Logging;

namespace ASTRedux.Utils;

/// <summary>
/// A wrapper class to cleanly read values at a specific offset in a file when using object initializers.
/// </summary>
internal static class PositionReader
{
    /// <summary>
    /// Sets the position of a referenced BinaryReader to a given offset and fetches a 32-bit integer from that location.
    /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
    /// </summary>
    /// <param name="reader">A BinaryReader object containing the desired file stream</param>
    /// <param name="offset">An offset (long) within the file to read from</param>
    /// <returns>A 32-bit integer from the specified stream location</returns>
    public static int ReadInt32At(BinaryReader reader, long offset, string inFile = "")
    {
        int output = 0;

        long restorePos = reader.BaseStream.Position;

        reader.BaseStream.Position = offset;

        output = reader.ReadInt32();

        reader.BaseStream.Position = restorePos;

        Logger.Message($"int (0x{output:X8}) read at offset 0x{offset:X8} {(inFile != string.Empty ? $"from file {Path.GetFileName(inFile)}" : string.Empty)}", LogType.INFO);

        return output;
    }

    /// <summary>
    /// Sets the position of a referenced BinaryReader to a given offset and fetches a 32-bit integer from that location.
    /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
    /// </summary>
    /// <param name="reader">A BinaryReader object containing the desired file stream</param>
    /// <param name="offset">An offset (long) within the file to read from</param>
    /// <returns>A 32-bit integer from the specified stream location</returns>
    public static uint ReadUInt32At(BinaryReader reader, long offset, string inFile = "")
    {
        uint output = 0;

        long restorePos = reader.BaseStream.Position;

        reader.BaseStream.Position = offset;

        output = reader.ReadUInt32();

        reader.BaseStream.Position = restorePos;

        Logger.Message($"uint (0x{output:X8}) read at offset 0x{offset:X8} {(inFile != string.Empty ? $"from file {Path.GetFileName(inFile)}" : string.Empty)}", LogType.INFO);

        return output;
    }

    /// <summary>
    /// Sets the position of a referenced BinaryReader to a given offset and fetches a 16-bit integer from that location.
    /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
    /// </summary>
    /// <param name="reader">A BinaryReader object containing the desired file stream</param>
    /// <param name="offset">An offset (long) within the file to read from</param>
    /// <returns>A 16-bit integer from the specified stream location</returns>
    public static short ReadInt16At(BinaryReader reader, long offset, string inFile = "")
    {
        short output = 0;

        long restorePos = reader.BaseStream.Position;

        reader.BaseStream.Position = offset;

        output = reader.ReadInt16();

        reader.BaseStream.Position = restorePos;

        Logger.Message($"short (0x{output:X4}) read at offset 0x{offset:X8} {(inFile != string.Empty ? $"from file {Path.GetFileName(inFile)}" : string.Empty)}", LogType.INFO);

        return output;
    }

    /// <summary>
    /// Sets the position of a referenced BinaryReader to a given offset and fetches an unsigned 16-bit integer from that location.
    /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
    /// </summary>
    /// <param name="reader">A BinaryReader object containing the desired file stream</param>
    /// <param name="offset">An offset (long) within the file to read from</param>
    /// <returns>A ushort from the specified stream location</returns>
    public static ushort ReadUInt16At(BinaryReader reader, long offset, string inFile = "")
    {
        ushort output = 0;

        long restorePos = reader.BaseStream.Position;

        reader.BaseStream.Position = offset;

        output = reader.ReadUInt16();

        reader.BaseStream.Position = restorePos;

        Logger.Message($"ushort (0x{output:X4}) read at offset 0x{offset:X8} {(inFile != string.Empty ? $"from file {Path.GetFileName(inFile)}" : string.Empty)}", LogType.INFO);

        return output;
    }

    public static byte[] ReadByteRange(BinaryReader reader, int startOffset, int endOffset)
    {
        int restorePos = (int)reader.BaseStream.Position;
        int length = endOffset - startOffset;

        reader.BaseStream.Position = startOffset;

        byte[] output = reader.ReadBytes(length);

        reader.BaseStream.Position = restorePos;

        return output;
    }
}
