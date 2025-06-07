using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTeroid.Structs
{
    /*
     * Header Format:
     * 0x00-0x03 = 32-bit magic
     * 
     */
    public struct ASTHeader
    {
        /// <summary>
        /// The magic number used by the AST format on LE platforms such as x86.
        /// </summary>
        public const uint LITTLE_ENDIAN_MAGIC = 0x4153544C;
        /// <summary>
        /// The magic number used by the AST format on BE platforms such as PPC.
        /// BE support not implemented.
        /// </summary>
        public const uint BIG_ENDIAN_MAGIC = 0x41535442;


        public ASTHeader()
        {
        }
    }
}
