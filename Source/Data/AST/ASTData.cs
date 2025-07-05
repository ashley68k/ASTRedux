using ASTRedux.Enums;
using ASTRedux.Data.Format;

namespace ASTRedux.Data.AST;

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
internal struct ASTData
{
    public Endian Endianness { get; set; }
    public int StartOffset { get; set; } 
    public int Length { get; set; }
    
    public AudioFormat Format { get; set; }
}
