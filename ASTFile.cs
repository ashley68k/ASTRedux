using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTeroid.Structs;

namespace ASTeroid
{
    internal class ASTFile
    {
        public ASTHeader Header { get; set; }
        public byte[]? FileContent { get; private set; }
        public byte[]? SampleBuffer { get; private set; }

        public const int MIN_FILE_SIZE = 0x40;

        public static ASTHeader ParseHeader()
        {
            // stub
            return new();
        }
    }
}
