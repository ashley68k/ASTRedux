﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Structs
{
    public static class StructExtensions
    {
        /// <summary>
        /// Converts a struct into a byte array for usage with BinaryWriter.
        /// </summary>
        /// <typeparam name="T">A generic type filtered for structs.</typeparam>
        /// <param name="input">A struct to be converted to a byte array</param>
        /// <returns>A byte array containing the memory layout of the struct</returns>
        public static byte[] ToByteArray<T>(this T input) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];

            nint ptr = Marshal.AllocHGlobal(size);
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
