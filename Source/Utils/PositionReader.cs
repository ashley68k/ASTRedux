using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTeroid.Utils
{
    /// <summary>
    /// A wrapper class to cleanly read values at a specific offset in a file when using object initializers.
    /// </summary>
    static class PositionReader
    {
        /// <summary>
        /// Sets the position of a referenced BinaryReader to a given offset and fetches a 32-bit integer from that location.
        /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
        /// </summary>
        /// <param name="reader">A BinaryReader object containing the desired file stream</param>
        /// <param name="offset">An offset (long) within the file to read from</param>
        /// <returns>A 32-bit integer from the specified stream location</returns>
        public static int ReadInt32At(BinaryReader reader, long offset)
        {
            reader.BaseStream.Position = offset;
            return reader.ReadInt32();
        }
        /// <summary>
        /// Sets the position of a referenced BinaryReader to a given offset and fetches a 16-bit integer from that location.
        /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
        /// </summary>
        /// <param name="reader">A BinaryReader object containing the desired file stream</param>
        /// <param name="offset">An offset (long) within the file to read from</param>
        /// <returns>A 16-bit integer from the specified stream location</returns>
        public static short ReadInt16At(BinaryReader reader, long offset)
        {
            reader.BaseStream.Position = offset;
            return reader.ReadInt16();
        }

        /// <summary>
        /// Sets the position of a referenced BinaryReader to a given offset and fetches an unsigned 16-bit integer from that location.
        /// Beware the position of the reader will NOT be reset, and objects are passed by reference, so this WILL affect the state of the passed reader.
        /// </summary>
        /// <param name="reader">A BinaryReader object containing the desired file stream</param>
        /// <param name="offset">An offset (long) within the file to read from</param>
        /// <returns>A ushort from the specified stream location</returns>
        public static ushort ReadUInt16At(BinaryReader reader, long offset)
        {
            reader.BaseStream.Position = offset;
            return reader.ReadUInt16();
        }
    }
}
