using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTeroid.Structs
{
    /*
     * Header Format:
     * Each entry is 4-byte wide and all are integers.
     * 0x00-0x03 = 32-bit magic (appears as a string in hex editor, but Dead Rising executable interprets the first 4 bytes of AST as an int and does an int compare to validate)
     * 0x04-0x07 = always 0x00000000?
     * 0x08-0x0B = unknown (always 01 02 00 00)
     * 0x0C-0x0F = unknown (1?)
     * 0x10-0x13 = music offset in file
     * 0x14-0x1F = 12 0x00 bytes?
     * 0x20-0x23 = length of music (offset + length)
     * 0x24-0x2F = 12 0xFF bytes?
     * 0x30-0x31 = 1 (maybe a flag?)
     * 0x32-0x33 = channel count
     * 0x34-0x37 = sample rate
     * 0x38-0x3B = bytes per second (sampleRate * bitDepth * channels) / 8
     * 0x3C-0x3D = unknown (always 4)
     * 0x3E-0x3F = bit-depth
     */
    public struct ASTHeader
    {
        /// <summary>
        /// The magic number (ASTL) used by the AST format on LE platforms such as x86.
        /// Little Endian AST files are similar to .wav files, and use raw PCM. rSoundAst files in the .arc archive have much more complicated structures.
        /// Position 0x00 - 0x03
        /// </summary>
        public const uint LITTLE_ENDIAN_MAGIC = 0x4153544C;
        /// <summary>
        /// The magic number (ASTB) used by the AST format on BE platforms such as PPC.
        /// BE support not implemented. Big Endian AST files, as in Dead Rising on the Xbox 360, use XMA encoding.
        /// Position 0x00 - 0x03
        /// </summary>
        public const uint BIG_ENDIAN_MAGIC = 0x41535442;

        public uint StartOffset;
        public uint ASTLength;
        public uint SampleRate;
        public uint BitDepth;
        public uint BytesPerSecond;
        public ushort Channels;
        public ushort Flag;

        public const uint ZERO_PAD = 0x00000000;
        public const uint FFFF_PAD = 0xFFFFFFFF;
    }
}
