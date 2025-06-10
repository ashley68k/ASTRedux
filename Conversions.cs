using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTeroid
{
    public static class Conversions
    {
        /// <summary>
        /// Converts a struct into a byte array for usage with BinaryWriter.
        /// </summary>
        /// <typeparam name="T">A generic type filtered for structs.</typeparam>
        /// <param name="input">A struct to be converted to a byte array</param>
        /// <returns>A byte array containing the memory layout of the struct</returns>
        public static byte[] StructToByteArray<T>(this T input) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(input, ptr, false);
                Marshal.Copy(ptr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return buffer;
        }
    }
}
