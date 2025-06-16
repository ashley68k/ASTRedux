using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ASTRedux.Structs.CSound.Sub;
using ASTRedux.Utils;

namespace ASTRedux.Structs.CSound
{
    internal struct CSoundHeader
    {
        public const int FILE_MAGIC = 0x4C444E53; // sndl (sound, little endian)
        public int FileLength { get; private set; } // file length + 1
        
        public int Unknown1 { get; private set; } // 512?

        public uint FFPad_1 = Constants.FFFF_PAD;

        public int CSROffset { get; private set; }

        public int CSHOffset { get; private set; }
        public int CSBOffset { get; private set; }

        public uint FFPad_2 = Constants.FFFF_PAD;

        // unimplemented for now
       /* CSRHead csr = new();
        List<CSREntry> csrEntries = new List<CSREntry>();

        CSHHead csh = new();
        List<CSHEntry> cshEntries = new List<CSHEntry>();

        CSBHead csb = new();
        List<CSBEntry> csbEntries = new List<CSBEntry>(); */

        public CSoundHeader()
        {
        }
    }
}
