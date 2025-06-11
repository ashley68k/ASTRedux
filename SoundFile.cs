using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTeroid.Structs.CSound.Sub;

namespace ASTeroid
{
    // to make this more extendable, in the future we should consider having ast and sound inherit from a base class and further abstract everything
    public class SoundFile
    {
        public const int FILE_MAGIC = 0x4C444E53; // sndl (sound, little endian)
        public const int CSR_MAGIC = 0x20525343; // csr (CSoundResource?)
        public const int CSH_MAGIC = 0x20485343; // csh (CSoundHeader?)
        public const int CSB_MAGIC = 0x20425343; // csb (CSoundBank?)
        public const int MAX_HEADER_SIZE = 0x800;

        CSR csr = new();
        CSH csh = new();
        CSB csb = new();
    }
}
