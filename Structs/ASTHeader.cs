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
     * 0x00-0x03 = int magic (appears as a string in hex editor, but Dead Rising executable interprets the first 4 bytes of AST as an int and does an int compare to validate) 
     * 0x04-0x07 = always 0x00000000? 
     * 0x08-0x0B = unknown (always 01 02 00 00) (int)
     * 0x0C-0x0F = unknown (1?) (int)
     * 0x10-0x13 = music offset in file (int)
     * 0x14-0x1F = 12 0x00 bytes?
     * 0x20-0x23 = length of music (int)
     * 0x24-0x2F = 12 0xFF bytes?
     * 0x30-0x31 = PCM flag such as in .wav header? (short)
     * 0x32-0x33 = channel count (short)
     * 0x34-0x37 = sample rate (int)
     * 0x38-0x3B = bytes per second (sampleRate * bitDepth * channels) / 8 (int)
     * 0x3C-0x3D = unknown (always 4) (short)
     * 0x3E-0x3F = bit-depth (short)
     */
    public struct ASTHeader
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

        public int StartOffset { get; set; } 
        public int ASTLength { get; set; }
        public int SampleRate { get; set; }
        public short BitDepth { get; set; }
        public int BytesPerSecond { get; set; }
        public short Channels { get; set; }
        public short PCMFlag { get; set; }

        public const uint ZERO_PAD = 0x00000000;
        public const uint FFFF_PAD = 0xFFFFFFFF;
    }
}
