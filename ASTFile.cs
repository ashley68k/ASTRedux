using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ASTeroid.Structs;

namespace ASTeroid
{
    internal class ASTFile
    {
        public ASTHeader Header { get; set; }
        public byte[]? FileContent { get; private set; }
        public byte[]? SampleBuffer { get; private set; }

        public const int MIN_FILE_SIZE = 0x40;

        /// <summary>
        /// Returns the expected file size of an AST object by adding audio length to audio start offset.
        /// </summary>
        /// <returns>Expected file size, or -1 on invalid length. Length check isn't rigorous.</returns>
        public int GetFileSize()
        {
            int length = Header.StartOffset + Header.ASTLength;
            return length >= MIN_FILE_SIZE ? length : -1;
        }

        /// <summary>
        /// Compares the integer magic in an AST stream versus the expected magic
        /// </summary>
        /// <param name="reader">A BinaryReader containing the data of an AST file</param>
        /// <returns>True on LE/BE AST match, false otherwise</returns>
        public static bool ValidateMagic(BinaryReader reader)
        {
            int streamMagic = PositionReader.ReadInt32At(reader, 0x00);
            return streamMagic == ASTHeader.LITTLE_ENDIAN_MAGIC || streamMagic == ASTHeader.BIG_ENDIAN_MAGIC;
        }

        /// <summary>
        /// Creates an ASTHeader struct from an input stream.
        /// </summary>
        /// <param name="fileStream">A stream containing the binary data of an AST file</param>
        /// <returns>An ASTHeader matching the input.</returns>
        public static ASTHeader ParseHeader(Stream fileStream)
        {
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                return new ASTHeader
                {
                    StartOffset = PositionReader.ReadInt32At(reader, 0x10),
                    ASTLength = PositionReader.ReadInt32At(reader, 0x20),
                    Flag = PositionReader.ReadInt16At(reader, 0x30),
                    Channels = PositionReader.ReadInt16At(reader, 0x32),
                    BytesPerSecond = PositionReader.ReadInt32At(reader, 0x38),
                    BitDepth = PositionReader.ReadInt16At(reader, 0x3E),
                    SampleRate = PositionReader.ReadInt32At(reader, 0x40)  
                };
            }
        }
    }
}
