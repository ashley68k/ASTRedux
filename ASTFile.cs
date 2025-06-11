using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ASTeroid.Enums;
using ASTeroid.Structs.AST;
using ASTeroid.Utils;
using NAudio.Wave;

namespace ASTeroid
{
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

        public const int MAX_HEADER_SIZE = 0x800;

        public ASTData AudioInfo { get; set; }

        public ASTHeader Header { get; set; }

        /// <summary>
        /// Compares the integer magic in an AST stream versus the expected magic
        /// </summary>
        /// <param name="reader">A BinaryReader containing the data of an AST file</param>
        /// <returns>True on LE/BE AST match, false otherwise</returns>
        public static bool ValidateMagic(BinaryReader reader)
        {
            int streamMagic = PositionReader.ReadInt32At(reader, 0x00);
            return streamMagic == LITTLE_ENDIAN_MAGIC || streamMagic == BIG_ENDIAN_MAGIC;
        }

        /// <summary>
        /// Creates an AudioData object from an input AST.
        /// </summary>
        /// <param name="fileStream">A stream containing the binary data of an AST file</param>
        /// <returns>An AudioData object matching the input.</returns>
        public static ASTData ParseData(BinaryReader reader)
        {
            return new ASTData
            {
                StartOffset = PositionReader.ReadInt32At(reader, 0x10),
                Length = PositionReader.ReadInt32At(reader, 0x20),
                PCMFlag = PositionReader.ReadInt16At(reader, 0x30),
                Channels = PositionReader.ReadInt16At(reader, 0x32),
                SampleRate = PositionReader.ReadInt32At(reader, 0x34),
                BytesPerSecond = PositionReader.ReadInt32At(reader, 0x38),
                BitDepth = PositionReader.ReadInt16At(reader, 0x3E),
                // always LE until BE support is added
                Endianness = Endian.LITTLE_ENDIAN
            };
        }

        /// <summary>
        /// Creates an AudioData object from an input audio file.
        /// </summary>
        /// <param name="fileStream">A stream containing the binary data of an audio file</param>
        /// <returns>An AudioData object matching the input.</returns>
        public static ASTData ParseData(Wave32To16Stream reader, int length)
        {
            WaveFormat fmt = reader.WaveFormat;
            return new ASTData
            {
                // dead rising ASTs always start at 0x800, and header is 0x40 anyways. length and PCM is also guaranteed
                StartOffset = 0x800,
                Length = length,
                PCMFlag = 1,
                Channels = (short)fmt.Channels,
                BytesPerSecond = (fmt.SampleRate * fmt.BitsPerSample * fmt.Channels) / 8,
                BitDepth = (short)fmt.BitsPerSample,
                SampleRate = fmt.SampleRate,
                BlockSize = (short)(fmt.Channels * (fmt.BitsPerSample / 8)),
                // always LE until BE support is added
                Endianness = Endian.LITTLE_ENDIAN
            };
        }

        public ASTFile(ASTData inData)
        {
            AudioInfo = inData;
        }
    }
}
