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
        public const int MAX_HEADER_SIZE = 0x800;
    }
}
