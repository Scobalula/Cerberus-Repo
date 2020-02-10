// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: IO/BinaryWriterExtensions.cs
// Author: Philip/Scobalula
// Description: BinaryReader extensions for writing null terminated strings, structures, etc.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PhilLibX.IO
{
    /// <summary>
    /// IO Utilities/Extensions
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes a null terminated string
        /// </summary>
        /// <param name="bw">Output Stream</param>
        /// <param name="value">Value to write</param>
        public static void WriteNullTerminatedString(this BinaryWriter bw, string value)
        {
            bw.Write(Encoding.UTF8.GetBytes(value));
            bw.Write((byte)0);
        }

        /// <summary>
        /// Writes a UTF16 null terminated string
        /// </summary>
        /// <param name="bw">Output Stream</param>
        /// <param name="value">Value to write</param>
        public static void WriteNullTerminatedUTF16String(this BinaryWriter bw, string value)
        {
            bw.Write(Encoding.Unicode.GetBytes(value));
            bw.Write((byte)0);
        }

        /// <summary>
        /// Writes the given structure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bw"></param>
        /// <param name="value"></param>
        public static void WriteStruct<T>(this BinaryWriter bw, T value)
        {
            bw.Write(Bytes.StructToBytes(value));
        }
    }
}
