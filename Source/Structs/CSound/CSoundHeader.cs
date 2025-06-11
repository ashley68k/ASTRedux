using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTeroid.Structs.CSound.Sub;

namespace ASTeroid.Structs.CSound
{
    internal struct CSoundHeader
    {
        public const int FILE_MAGIC = 0x4C444E53; // sndl (sound, little endian)
        public int FileLength { get; private set; } // file length + 1

        CSRHead csr = new();
        List<CSREntry> csrEntries = new List<CSREntry>();

        CSHHead csh = new();
        List<CSHEntry> cshEntries = new List<CSHEntry>();

        CSBHead csb = new();
        List<CSBEntry> csbEntries = new List<CSBEntry>();

        public CSoundHeader()
        {
        }
    }
}
