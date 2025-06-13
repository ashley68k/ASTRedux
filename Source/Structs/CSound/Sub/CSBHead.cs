using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Structs.CSound.Sub
{
    internal struct CSBHead
    {
        public const int CSB_MAGIC = 0x20425343; // csb (CSoundBank?)
        public int fileLengthFromCSB { get; set; } // (file length - csb offset) + 1
        public int numSounds { get; set; } // number of sounds in file
    }
}
