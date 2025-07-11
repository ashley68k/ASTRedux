using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils.Consts
{
    internal static class rSndOffsets
    {
        // sndl
        public static int FileSize = 0x04;
        public static int CSRPtrPosition = 0x10;
        public static int CSHPtrPosition = 0x14;
        public static int CSBPtrPosition = 0x18;

        // csb
        public static int CSBRelWaveNo = 0x08;
    }
}
