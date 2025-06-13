using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Structs.Format
{
    // directly analogous to WAVEFORMAT
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SampleFormat
    {
        public ushort FormatFlag { get; set; }
        public short Channels { get; set; }
        public int SampleRate { get; set; }
        public int BytesPerSecond { get; set; }
        public short BlockSize { get; set; }
        public short BitDepth { get; set; }

        public SampleFormat(ushort formatFlag, short channels, int sampleRate, int bps, short blockSize, short bitDepth) 
        { 
            FormatFlag = formatFlag;
            Channels = channels;
            SampleRate = sampleRate;
            BytesPerSecond = bps;
            BlockSize = blockSize;
            BitDepth = bitDepth;
        }
    }
}
