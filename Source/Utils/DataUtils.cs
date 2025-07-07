using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils
{
    internal static class DataUtils
    {
        private static long RoundToNearestMultiple(long num, long inMul)
        {
            return ((num + inMul / 2) / inMul) * inMul;
        }

        /// <summary>
        /// Returns the closest multiple of 0x800 or 0x1000 from an offset.
        /// </summary>
        /// <param name="offset">A long representing a file position which must be rounded to an rSound alignment boundary.</param>
        /// <returns>The nearest free rSound alignment as an offset</returns>
        public static long GetRSoundAlignment(long offset)
        {
            long x = RoundToNearestMultiple(offset, 0x800);
            long y = RoundToNearestMultiple(offset, 0x1000);

            long distX = Math.Abs(offset - x);
            long distY = Math.Abs(offset - y);

            return distX < distY ? x : y;
        }
    }
}
