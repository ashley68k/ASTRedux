using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ASTRedux.Enums;
using ASTRedux.Structs.Format;

namespace ASTRedux.Structs.AST
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
     * I suspect the rest is some format identifier, guid block, etc. Regardless it's all same between files despite one value which changes nothing when altered. This section should still be reversed more.
     * 
     * Packed this way we can just create an ASTHeader and send it off.
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ASTHeader
    {
        public int Magic { get; private set; }
        public readonly int ZeroPad_1 = 0x00000000;
        public readonly int Unknown_1 = 0x00000201;
        public readonly int Unknown_2 = 0x00000001;
        public int AudioOffset { get; private set; }
        public readonly int ZeroPad_2 = 0x00000000;
        public readonly int ZeroPad_3 = 0x00000000;
        public readonly int ZeroPad_4 = 0x00000000;
        public int AudioLength { get; private set; }
        public readonly uint FFFFPad_1 = 0xFFFFFFFF;
        public readonly uint FFFFPad_2 = 0xFFFFFFFF;
        public readonly uint FFFFPad_3 = 0xFFFFFFFF;
        public SampleFormat Format { get; set; }
        public short Unknown_3 { get; private set; }

        public readonly int Block_1 = 0xC0000;
        public readonly int Block_2 = 0x42F001;
        public readonly int Block_3 = 0xC;
        public readonly int Block_4 = 0x77953145;
        public readonly short Block_5 = 0x0;
        public readonly short Block_6 = 0x1FCB;
        public readonly int Block_7 = -2;

        public ASTHeader(ASTData data)
        {
            Magic = data.Endianness == Endian.LITTLE_ENDIAN ? ASTFile.LITTLE_ENDIAN_MAGIC : ASTFile.BIG_ENDIAN_MAGIC;
            AudioOffset = data.StartOffset;
            AudioLength = data.Length;

            // just copy the astdata format
            Format = data.Format;
        }
    }
}
